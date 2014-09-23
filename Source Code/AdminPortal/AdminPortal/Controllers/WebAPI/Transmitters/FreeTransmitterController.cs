using AdminPortal.Models;

using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Transmitters {

	/**
	 * Deregisters a transmitter from the system.
	 */
	public class FreeTransmitterController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult FreeTransmitter(RequestDetails details) {
			int id = int.Parse(details.ID);
			var transmitter = Db.Transmitters.Find(id);

			if (transmitter == null) {
				return BadRequest();
			}

			Db.Transmitters.Remove(transmitter);
			Db.SaveChanges();

			return Ok();
		}

		public struct RequestDetails {
			public string ID;
		}
	}
}