using System;
using System.Collections.Generic;
using System.Threading;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

using Transmitter.Utility;

using no.nordicsemi.android.beacon;

namespace Transmitter {

	[Activity(Label = "TransmitActivity")]			
	public class TransmitActivity : Activity {
		public const int TRANSMIT_FREQUENCY = 1000;

		EditText transmitterIdBox, beaconCountBox, updateCountBox;
		Button stopButton;
		string id;
		long lastSent = 0;
		int updates = 0;
		List<String> prevBeacons = new List<string>();
		//Dictionary<string, float> TagData = new Dictionary<string, float>();

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Transmit);

			BluetoothAdapter ba = BluetoothAdapter.DefaultAdapter;

			if (ba == null) {
				Toast.MakeText(this, "Bluetooth not supported", ToastLength.Short).Show();
				Finish();
				return;
			}

			if (!ba.IsEnabled) {
				var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
				StartActivityForResult(enableBtIntent, 1);
			}

			id = Intent.GetStringExtra("ID");
			mConnection = new MyBeaconServiceConnection(this);
			mBeaconsListener = new MyBeaconsListener(this);
			Window.AddFlags(WindowManagerFlags.KeepScreenOn);

			transmitterIdBox = FindViewById<EditText>(Resource.Id.TransmitterId);
			beaconCountBox = FindViewById<EditText>(Resource.Id.BeaconCount);
			updateCountBox = FindViewById<EditText>(Resource.Id.UpdateCount);
			stopButton = FindViewById<Button>(Resource.Id.StopButton);

			stopButton.Click += (sender, e) => Finish(); 

			transmitterIdBox.Text = id;
		}

		protected override void OnDestroy() {
			base.OnDestroy();

			var jb = new JsonBuilder();
			jb.AddEntry("ID", id);

			AppTools.SendRequest("api/freetransmitter", jb.ToString());
		}

		protected override void OnResume() {
			base.OnResume();

			ServiceProxy.bindService(this, mConnection);
			ServiceProxy.stopMonitoringForRegion(this, mMyUuid);
		}

		protected override void OnPause() {
			base.OnPause();

			// Intent to launch when the notification is pressed:
			var intent = new Intent(this, typeof(MainActivity));
			intent.SetFlags(ActivityFlags.NewTask);

			// Create a pending intent
			var pendingIntent = PendingIntent.GetActivity(this, 1, intent, PendingIntentFlags.UpdateCurrent);
			// Create and configure the notification
			var builder = new Notification.Builder(this); // the notification icon (small icon) will be overwritten by the BeaconService.
			builder.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.Icon)).SetContentTitle("Beacon is in range!").SetContentText("Click to open app.");
			builder.SetAutoCancel(true).SetOnlyAlertOnce(true).SetContentIntent(pendingIntent);
			// Start monitoring for the region
			ServiceProxy.startMonitoringForRegion(this, mMyUuid, builder.Build());

			//mConnection.StopMonitoringForRegion(mRegionListener);
			mConnection.StopRangingBeaconsInRegion(mBeaconsListener);
			ServiceProxy.unbindService(this, mConnection);
		}

		static Java.Util.UUID mMyUuid = Java.Util.UUID.FromString("01122334-4556-6778-899A-ABBCCDDEEFF0");
		static Java.Util.UUID mAnyUuid = BeaconRegion.ANY_UUID;
		BeaconsListener mBeaconsListener;
		BeaconServiceConnection mConnection;

		class MyBeaconServiceConnection : BeaconServiceConnection {
			TransmitActivity Parent;

			public MyBeaconServiceConnection(TransmitActivity parent) {
				Parent = parent;
			}

			public override void OnServiceConnected() {
				StartRangingBeaconsInRegion(mMyUuid, Parent.mBeaconsListener);
				StartRangingBeaconsInRegion(mMyUuid, 5, Parent.mBeaconsListener);
				StartRangingBeaconsInRegion(mAnyUuid, Parent.mBeaconsListener);
			}

			public override void OnServiceDisconnected() {
			}
		}

		class MyBeaconsListener : BeaconsListener {
			TransmitActivity Parent;

			public MyBeaconsListener(TransmitActivity parent) {
				Parent = parent;
			}

			void BeaconsListener.OnBeaconsInRegion(Beacon[] beacons, BeaconRegion region) {
				Parent.beaconCountBox.Text = beacons.Length + "";
				Parent.updateCountBox.Text = Parent.updates + "";
				long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

				if (now - Parent.lastSent < TRANSMIT_FREQUENCY)
					return;

				var newBeacons = new List<string>();
				Parent.lastSent = now;
				var request = new Request();

				request.TransmitterId = Parent.id;

				foreach (var beacon in beacons) {
					var strength = new Strength();
					strength.TagMinorMajor = beacon.getMajor() + "-" + beacon.getMinor();
					strength.Distance = beacon.getAccuracy();

					newBeacons.Add(strength.TagMinorMajor);
					request.TagData.Add(strength);
				}

				foreach (var prev in Parent.prevBeacons) {
					if (!newBeacons.Contains(prev)) {
						var jb = new JsonBuilder();
						jb.AddEntry("BeaconId", prev);
						ThreadPool.QueueUserWorkItem(state => AppTools.SendRequest("api/outofrange", jb.ToString()));
					}
				}

				Parent.prevBeacons = newBeacons;

				ThreadPool.QueueUserWorkItem(state => {
					var response = AppTools.SendRequest("api/transmit", Serialiser<Request>.Run(request), TRANSMIT_FREQUENCY - 100);

					if (response.StatusCode == System.Net.HttpStatusCode.OK)
						Parent.updates++;
				});
			}

			class Request {
				public String TransmitterId = "";
				public List<Strength> TagData = new List<Strength>();
			}

			class Strength {
				public String TagMinorMajor = "";
				public float Distance = 0.0f;
			}
		}
	}
}
