using System.Collections.Generic;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	public class TransmitController : ApiController {
		
		[HttpPost]
		public IHttpActionResult Transmit(List<RequestDetails> details) {
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