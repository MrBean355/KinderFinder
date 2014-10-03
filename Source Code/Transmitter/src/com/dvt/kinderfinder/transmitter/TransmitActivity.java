package com.dvt.kinderfinder.transmitter;

import java.util.LinkedList;
import java.util.UUID;

import no.nordicsemi.android.beacon.Beacon;
import no.nordicsemi.android.beacon.BeaconRegion;
import no.nordicsemi.android.beacon.BeaconServiceConnection;
import no.nordicsemi.android.beacon.ServiceProxy;
import android.annotation.TargetApi;
import android.app.Notification;
import android.app.PendingIntent;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import android.graphics.BitmapFactory;
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

@TargetApi(Build.VERSION_CODES.JELLY_BEAN) public class TransmitActivity extends ActionBarActivity {
	private static final int TRANSMIT_FREQUENCY = 500;
	
	private long lastSent;
	private int updates = 0;
	private String id;
	private EditText transmitterIdBox, beaconCountBox, updateCountBox;
	private Button stopButton;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_transmit);

		id = getIntent().getStringExtra("ID");
		getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
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

		transmitterIdBox = (EditText) findViewById(R.id.idBox);
		beaconCountBox = (EditText) findViewById(R.id.countBox);
		updateCountBox = (EditText) findViewById(R.id.updatesBox);
		stopButton = (Button) findViewById(R.id.stopButton);
		
		stopButton.setOnClickListener(buttonHandler);
		
		transmitterIdBox.setText(id);
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
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
		public String TagUuid;
		public float Distance;
	}
	
	/*private BeaconServiceConnection.RegionListener mRegionListener = new BeaconServiceConnection.RegionListener() {
		@Override
		public void onEnterRegion(final BeaconRegion region) {
			//Log.i(TAG, "onEnterRegion: " + region);
		}

		@Override
		public void onExitRegion(final BeaconRegion region) {
			//Log.i(TAG, "onExitRegion: " + region);
		}
	};*/
	
	private BeaconServiceConnection.BeaconsListener mBeaconsListener = new BeaconServiceConnection.BeaconsListener() {
		@Override
		public void onBeaconsInRegion(final Beacon[] beacons, final BeaconRegion region) {
			// The following snippet ensures that we only send to the server
			// at most once per second. This prevents the server from being
			// flooded with data (hopefully).
			long now = System.currentTimeMillis();
			long diff = now - lastSent;
			
			if (diff < TRANSMIT_FREQUENCY) {
				return;
			}
			else {
				lastSent = now;
			}
			
			Request req = new Request();
			req.TransmitterId = id;
			req.TagData = new LinkedList<>();
			
			for (final Beacon beacon : beacons) {
				Strength str = new Strength();
				str.TagUuid = beacon.getMajor() + "-" + beacon.getMinor();
				str.Distance = beacon.getAccuracy();
				
				req.TagData.add(str);
			}

			new RequestTask().execute("api/transmit", new Gson().toJson(req));
			
			updates++;
			
			beaconCountBox.setText(beacons.length + "");
			updateCountBox.setText(updates + "");
		}
	};

	private BeaconServiceConnection mConnection = new BeaconServiceConnection() {
		@Override
		public void onServiceConnected() {
			//startMonitoringForRegion(mAnyUuid, mRegionListener);
			startRangingBeaconsInRegion(mMyUuid, mBeaconsListener);
			startRangingBeaconsInRegion(mMyUuid, 5, mBeaconsListener);
			startRangingBeaconsInRegion(mAnyUuid, mBeaconsListener);
		}

		@Override
		public void onServiceDisconnected() {
			
		}
	};
	
	private OnClickListener buttonHandler = new OnClickListener() {
		@Override
		public void onClick(View v) {
			finish();
		}
	};
}
