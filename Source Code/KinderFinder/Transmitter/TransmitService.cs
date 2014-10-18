using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using Transmitter.Utility;
using no.nordicsemi.android.beacon;

namespace Transmitter {

	[Service]
	public class TransmitService : Service {
		public const int TRANSMIT_FREQUENCY = 1000;

		static bool Running = false;

		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;

		string TransmitterId;

		public override void OnCreate() {
			base.OnCreate();
			Running = true;

			Pref = GetSharedPreferences("Preferences", 0);
			Editor = Pref.Edit();

			// Initialise beacon service:
			mConnection = new MyBeaconServiceConnection(this);
			mBeaconsListener = new MyBeaconsListener(this);
			ServiceProxy.bindService(this, mConnection);

			Console.WriteLine("--- Service created!");
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
			base.OnStartCommand(intent, flags, startId);

			TransmitterId = Pref.GetString("ID", "");

			if (TransmitterId.Equals("")) {
				Toast.MakeText(this, "Unable to load ID; start app again", ToastLength.Long).Show();
				StopSelf();
			}

			Console.WriteLine("--- Service started!");

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy() {
			base.OnDestroy();
			Running = false;

			// Stop scanning for beacons:
			mConnection.StopRangingBeaconsInRegion(mBeaconsListener);
			ServiceProxy.unbindService(this, mConnection);

			// Tell the server we're done:
			var jb = new JsonBuilder();
			jb.AddEntry("ID", TransmitterId);
			AppTools.SendRequest("api/freetransmitter", jb.ToString());

			Console.WriteLine("--- Service destroyed!");
		}

		public override IBinder OnBind(Intent intent) {
			return null;
		}

		public static bool IsRunning() {
			return Running;
		}

		class Request {
			public String TransmitterId = "";
			public List<Strength> TagData = new List<Strength>();

			public class Strength {
				public String TagMinorMajor = "";
				public float Distance = 0.0f;
			}
		}

		//static Java.Util.UUID mMyUuid = Java.Util.UUID.FromString("01122334-4556-6778-899A-ABBCCDDEEFF0");
		//static Java.Util.UUID mAnyUuid = BeaconRegion.ANY_UUID;
		BeaconsListener mBeaconsListener;
		BeaconServiceConnection mConnection;

		class MyBeaconServiceConnection : BeaconServiceConnection {
			TransmitService Parent;

			public MyBeaconServiceConnection(TransmitService parent) {
				Parent = parent;
			}

			public override void OnServiceConnected() {
				//StartRangingBeaconsInRegion(mMyUuid, Parent.mBeaconsListener);
				//StartRangingBeaconsInRegion(mMyUuid, 5, Parent.mBeaconsListener);
				StartRangingBeaconsInRegion(BeaconRegion.ANY_UUID, Parent.mBeaconsListener);
			}

			public override void OnServiceDisconnected() {
			}
		}

		class MyBeaconsListener : BeaconsListener {
			readonly TransmitService Parent;
			long LastSent = 0;
			List<string> PrevBeacons = new List<string>();

			public MyBeaconsListener(TransmitService parent) {
				Parent = parent;
			}

			void BeaconsListener.OnBeaconsInRegion(Beacon[] beacons, BeaconRegion region) {
				long now = AppTools.GetCurrentTime();
				long elapsed = now - LastSent;

				if (elapsed < 1000)
					return;

				LastSent = now;

				var request = new Request();
				request.TransmitterId = Parent.TransmitterId;

				// Construct the request to send to the server (update of strengths):
				foreach (var beacon in beacons) {
					var strength = new Request.Strength();
					strength.TagMinorMajor = beacon.getMajor() + "-" + beacon.getMinor();
					strength.Distance = beacon.getAccuracy();

					request.TagData.Add(strength);
				}

				AppTools.SendRequest("api/transmit", Serialiser<Request>.Run(request), TRANSMIT_FREQUENCY);

				// Check whether any beacons are out of range. This is done by checking each previously detected beacon
				// (in PrevBeacons) to see if it was detected now (whether it's in beacons).
				foreach (var prev in PrevBeacons) {
					bool found = false;

					foreach (var beacon in beacons) {
						string id = beacon.getMajor() + "-" + beacon.getMinor();

						if (id.Equals(prev)) {
							found = true;
							break;
						}
					}

					// Beacon was not detected again; tell the server:
					if (!found) {
						var jb = new JsonBuilder();
						jb.AddEntry("BeaconId", prev);
						AppTools.SendRequest("api/outofrange", jb.ToString());
					}
				}

				// Finally, we need to update the list of previously detected beacons to be the same as the currently
				// detected beacons. This ensures we only tell the server about out of range beacons once.
				PrevBeacons.Clear();

				foreach (var beacon in beacons) {
					string id = beacon.getMajor() + "-" + beacon.getMinor();
					PrevBeacons.Add(id);
				}
			}
		}
	}
}
