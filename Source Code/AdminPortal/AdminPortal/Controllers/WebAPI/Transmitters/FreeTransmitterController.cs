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
			System.Diagnostics.Debug.WriteLine("Trying to free transmitter " + id);

			if (transmitter == null) {
				System.Diagnostics.Debug.WriteLine("\tDoesn't exist!");
				return BadRequest();
			}

			System.Diagnostics.Debug.WriteLine("\tSuccess!");
			Db.Transmitters.Remove(transmitter);
			Db.SaveChanges();

			return Ok();
		}

		public struct RequestDetails {
			public string ID;
		}
	}
}