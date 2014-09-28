using AdminPortal.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace AdminPortal.Controllers.WebAPI.Transmitters {

	/**
	 * Receives a list of beacons and strengths and processes it.
	 */
	public class TransmitController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult Transmit(RequestDetails details) {
			// Determine whether the transmitter is still registered:
			int id = int.Parse(details.TransmitterId);

			var transmitter = (from item in Db.Transmitters
							   where item.ID == id
							   select item).FirstOrDefault();

			if (transmitter == null) {
				return BadRequest(); // transmitter not in system.
			}

			//StrengthManager.Update(transmitter.ID, (int)transmitter.Type, -FeetToMeters(details.TagData[0].Distance));

			foreach (var item in details.TagData) {
				StrengthManager.Update(item.TagUuid, id, (int)transmitter.Type, item.Distance);
			}

			return Ok();
		}

		private double FeetToMeters(double feet) {
			return feet * 0.3048;
		}

		public struct RequestDetails {
			public string TransmitterId;
			public List<Strength> TagData;
		}
	}
}