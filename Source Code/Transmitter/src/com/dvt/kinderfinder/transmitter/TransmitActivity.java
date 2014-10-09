package com.dvt.kinderfinder.transmitter;

import java.util.LinkedList;
import java.util.UUID;

import no.nordicsemi.android.beacon.Beacon;
import no.nordicsemi.android.beacon.BeaconRegion;
import no.nordicsemi.android.beacon.BeaconServiceConnection;
import no.nordicsemi.android.beacon.ServiceProxy;

import org.apache.http.HttpStatus;

import android.app.Notification;
import android.app.PendingIntent;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.google.gson.Gson;

public class TransmitActivity extends ActionBarActivity {
	private static final int TRANSMIT_FREQUENCY = 1000;
	
	private long lastSent;
	private int updates = 0;
	private String id;
	private EditText transmitterIdBox, beaconCountBox, updateCountBox;
	private Button stopButton;
	private LinkedList<String> prevBeacons;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_transmit);

		getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON); // tell system to keep screen on.
		
		// Check if Bluetooth is on, and ask to switch on it it's off:
		BluetoothAdapter ba = BluetoothAdapter.getDefaultAdapter();
		
		if (ba == null) {
		    Toast.makeText(getApplicationContext(), "Bluetooth not supported", Toast.LENGTH_LONG).show();
		    finish();
		    return;
		}

		if (!ba.isEnabled()) {
		    Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
		    startActivityForResult(enableBtIntent, 1);
		}

		id = getIntent().getStringExtra("ID"); // load our unique ID.
		prevBeacons = new LinkedList<String>();
		
		transmitterIdBox = (EditText) findViewById(R.id.idBox);
		beaconCountBox = (EditText) findViewById(R.id.countBox);
		updateCountBox = (EditText) findViewById(R.id.updatesBox);
		stopButton = (Button) findViewById(R.id.stopButton);
		
		stopButton.setOnClickListener(buttonHandler);

		transmitterIdBox.setText(id);
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		// If we requested to switch on Bluetooth and the user rejected, close app:
		if (requestCode == 1 && resultCode != RESULT_OK) {
		    Toast.makeText(getApplicationContext(), "Must enable Bluetooth to continue", Toast.LENGTH_LONG).show();
		    finish();
		}
	}
	
	@Override
	protected void onResume() {
		super.onResume();

		lastSent = 0;
		ServiceProxy.bindService(this, mConnection);
		ServiceProxy.stopMonitoringForRegion(this, mMyUuid);
	}
	
	@Override
	protected void onDestroy() {
		super.onDestroy();
		
		JsonBuilder jb = new JsonBuilder();
		jb.addEntry("ID", id);
		new RequestTask().execute("api/freetransmitter", jb.toString());
	}
	
	@Override
	protected void onPause() {
		super.onPause();

		// This intent will be launched when user press the notification
		final Intent intent = new Intent(this, MainActivity.class);
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		// Create a pending intent
		final PendingIntent pendingIntent = PendingIntent.getActivity(this, 1, intent, PendingIntent.FLAG_UPDATE_CURRENT);
		// Create and configure the notification
		final Notification.Builder builder = new Notification.Builder(this); // the notification icon (small icon) will be overwritten by the BeaconService.
		builder.setLargeIcon(BitmapFactory.decodeResource(getResources(), R.drawable.ic_launcher)).setContentTitle("Beacon is in range!").setContentText("Click to open app.");
		builder.setAutoCancel(true).setOnlyAlertOnce(true).setContentIntent(pendingIntent);
		// Start monitoring for the region
		ServiceProxy.startMonitoringForRegion(this, mMyUuid, builder.build());

		//mConnection.stopMonitoringForRegion(mRegionListener);
		mConnection.stopRangingBeaconsInRegion(mBeaconsListener);
		ServiceProxy.unbindService(this, mConnection);
	}
	
	private UUID mMyUuid = UUID.fromString("01122334-4556-6778-899A-ABBCCDDEEFF0");
	private UUID mAnyUuid = BeaconRegion.ANY_UUID;

	@SuppressWarnings("unused")
	private class Request {
		public String TransmitterId;
		public LinkedList<Strength> TagData;
	}

	@SuppressWarnings("unused")
	private class Strength {
		public String TagMinorMajor;
		public float Distance;
	}
	
	private BeaconServiceConnection.BeaconsListener mBeaconsListener = new BeaconServiceConnection.BeaconsListener() {
		@Override
		public void onBeaconsInRegion(final Beacon[] beacons, final BeaconRegion region) {
			// The following snippet ensures that we only send to the server
			// at most once per second. This prevents the server from being
			// flooded with data (hopefully).
			long now = System.currentTimeMillis();
			long diff = now - lastSent;
			
			if (diff < TRANSMIT_FREQUENCY)
				return;
			else
				lastSent = now;
			
			final Request req = new Request();
			req.TransmitterId = id;
			req.TagData = new LinkedList<>();
			LinkedList<String> newBeacons = new LinkedList<String>();
			
			for (final Beacon beacon : beacons) {
				String tagId = beacon.getMajor() + "-" + beacon.getMinor();
				Strength str = new Strength();
				str.TagMinorMajor = tagId;
				str.Distance = beacon.getAccuracy();
				req.TagData.add(str);
				
				newBeacons.add(tagId);
			}
			
			// For each previously detected beacon:
			for (String prev : prevBeacons) {
				// If it was not detected this time, raise an alarm:
				if (!newBeacons.contains(prev)) {
					JsonBuilder jb = new JsonBuilder();
					jb.addEntry("BeaconId", prev);
					new RequestTask().execute("api/outofrange", jb.toString());
				}
			}
			
			prevBeacons = newBeacons;
			
			RequestTask t = new RequestTask() {
				@Override
				public void onPostExecute(String result) {
					// Only increase the count only if the update was received:
					if (statusCode == HttpStatus.SC_OK) {
						updates++;
						
						runOnUiThread(new Runnable() {
							@Override
							public void run() {
								updateCountBox.setText(updates + "");
							}
						});
					}
				}
			};
			
			if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB)
				t.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, "api/transmit", new Gson().toJson(req));
			else
				t.execute("api/transmit", new Gson().toJson(req));
			
			beaconCountBox.setText(beacons.length + "");
		}
	};

	private BeaconServiceConnection mConnection = new BeaconServiceConnection() {
		@Override
		public void onServiceConnected() {
			startRangingBeaconsInRegion(mMyUuid, mBeaconsListener);
			startRangingBeaconsInRegion(mMyUuid, 5, mBeaconsListener);
			startRangingBeaconsInRegion(mAnyUuid, mBeaconsListener);
		}

		@Override
		public void onServiceDisconnected() { }
	};
	
	private OnClickListener buttonHandler = new OnClickListener() {
		@Override
		public void onClick(View v) {
			finish();
		}
	};
}
