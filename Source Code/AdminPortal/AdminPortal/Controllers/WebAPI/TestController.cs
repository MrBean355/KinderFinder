using AdminPortal.Code;

using System.Timers;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI {

	/// <summary>
	/// This is a test controller that mocks a transmitter. It sends mock strengths for some tags.
	/// </summary>
	/*public class TestController : ApiController {
		const int TRANSMITTER_ID = 129;
		const int TRANSMITTER_TYPE = 3;

		static string[] BeaconIds = { "1-177", "1-209" };
		static double[] Strengths = { 0.3, 0.6 };
		static Timer TransmitTimer;

		static TestController() {
			TransmitTimer = new Timer(1000.0);
			TransmitTimer.Elapsed += OnTimerTick;
			TransmitTimer.Start();
		}

		private static void OnTimerTick(object sender, ElapsedEventArgs e) {
			string output = "[Info] Updated strengths from type " + TRANSMITTER_TYPE + ": ";

			for (int i = 0; i < BeaconIds.Length; i++ ) {
				StrengthManager.Update(BeaconIds[i], TRANSMITTER_ID, TRANSMITTER_TYPE, Strengths[i]);
				StrengthManager.FlagTag(BeaconIds[i], false);
				output += BeaconIds[i] + "(" + Strengths[i] + ") ";
			}

			//System.Diagnostics.Debug.WriteLine(output);
		}

		public IHttpActionResult Get() {
			return Ok("Transmitting");
		}
	}*/
}