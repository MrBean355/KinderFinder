using System.Collections.Generic;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	public class PingController : ApiController {

		[HttpPost]
		public IHttpActionResult Ping() {
			System.Diagnostics.Debug.WriteLine("Someone pinged");
			return Ok("Hello there");
		}
	}

	public class GetIdController : ApiController {
		public static List<int> TakenIds = new List<int>();
		private const int MAX_ID = 100;

		[HttpPost]
		public IHttpActionResult GetId() {
			int id = -1;

			for (id = 0; id <= MAX_ID; id++) {
				if (!TakenIds.Contains(id)) {
					TakenIds.Add(id);
					break;
				}
			}

			if (id == -1) {
				return BadRequest();
			}

			return Ok(id);
		}
	}

	public class ReleaseIdController : ApiController {

		[HttpPost]
		public IHttpActionResult ReleaseId(ReleaseRequestDetails details) {
			if (details.ID == null) {
				return BadRequest();
			}

			int id = int.Parse(details.ID);

			if (GetIdController.TakenIds.Contains(id)) {
				GetIdController.TakenIds.Remove(id);
				return Ok();
			}

			return BadRequest();
		}

		public struct ReleaseRequestDetails {
			public string ID;
		}
	}
}