using System;
using System.Collections.Generic;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;

using Transmitter.Utility;
using no.nordicsemi.android.beacon;
using Android.Widget;

namespace Transmitter {

	[Service]
	public class TransmitService : Service {
		public const int TRANSMIT_FREQUENCY = 1000;

		static bool Running = false;

		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;

		Timer TaskTimer;
		string TransmitterId;
		Dictionary<string, float> Strengths = new Dictionary<string, float>();

		public override IBinder OnBind(Intent intent) {
			return null;
		}

		public static bool IsRunning() {
			return Running;
		}

		public override void OnCreate() {
			base.OnCreate();
			Running = true;

			Pref = GetSharedPreferences("Preferences", 0);
			Editor = Pref.Edit();

			// Initialise beacon service:
			mConnection = new MyBeaconServiceConnection(this);
			mBeaconsListener = new MyBeaconsListener(this);
			ServiceProxy.bindService(this, mConnection);

			// Start timer ticking:
			TaskTimer = new Timer(OnTimerTick);
			TaskTimer.Change(TRANSMIT_FREQUENCY, TRANSMIT_FREQUENCY);

			Console.WriteLine("--- Service created!");

			Strengths.Add("1-199", 1.23f);
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
			base.OnStartCommand(intent, flags, startId);

			TransmitterId = Pref.GetString("ID", "");

			if (TransmitterId.Equals("")) {
				Toast.MakeText(this, "Unable to load ID; start app again", ToastLength.Long).Show();
				StopSelf();
			}

			Console.WriteLine("--- Service started! " + TransmitterId);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy() {
			base.OnDestroy();
			Running = false;

			// Stop scanning for beacons:
			mConnection.StopRangingBeaconsInRegion(mBeaconsListener);
			ServiceProxy.unbindService(this, mConnection);

			// Stop sending data:
			if (TaskTimer != null)
				TaskTimer.Dispose();

			// Tell the server we're done:
			var jb = new JsonBuilder();
			jb.AddEntry("ID", TransmitterId);
			AppTools.SendRequest("api/freetransmitter", jb.ToString());

			Console.WriteLine("--- Service destroyed!");
		}

		void OnTimerTick(object state) {
			var request = new Request();
			request.TransmitterId = TransmitterId;

			foreach (var strength in Strengths) {
				var str = new Request.Strength();
				str.TagMinorMajor = strength.Key;
				str.Distance = strength.Value;

				request.TagData.Add(str);
			}

			AppTools.SendRequest("api/transmit", Serialiser<Request>.Run(request), TRANSMIT_FREQUENCY);
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

			public MyBeaconsListener(TransmitService parent) {
				Parent = parent;
			}

			void BeaconsListener.OnBeaconsInRegion(Beacon[] beacons, BeaconRegion region) {
				foreach (var beacon in beacons) {
					var id = beacon.getMajor() + "-" + beacon.getMinor();

					if (Parent.Strengths.ContainsKey(id))
						Parent.Strengths[id] = beacon.getAccuracy();
					else
						Parent.Strengths.Add(id, beacon.getAccuracy());
				}

				Console.WriteLine("--- Found " + beacons.Length + " beacons!");

				/*Parent.beaconCountBox.Text = beacons.Length + "";
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
				});*/
			}
		}
	}
}

