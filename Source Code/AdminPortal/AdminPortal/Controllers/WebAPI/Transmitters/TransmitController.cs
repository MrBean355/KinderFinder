using System.Collections.Generic;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Transmitters {

	/**
	 * Receives a list of beacons and strengths and processes it.
	 */
	public class TransmitController : ApiController {

		[HttpPost]
		public IHttpActionResult Transmit(List<RequestDetails> details) {
			// TODO: Do something with received strengths.
			foreach (var item in details) {
				System.Diagnostics.Debug.WriteLine(item.id + ": " + item.distance);
			}

			return Ok();
		}

		public struct RequestDetails {
			public string id;
			public float distance;
		}
	}
}