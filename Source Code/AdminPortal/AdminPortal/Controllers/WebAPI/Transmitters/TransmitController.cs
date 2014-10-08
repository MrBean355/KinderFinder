using AdminPortal.Code;
using AdminPortal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

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

			// Find the transmitter that is sending:
			var transmitter = (from item in Db.Transmitters
							   where item.ID == id
							   select item).FirstOrDefault();

			// Transmitter not found:
			if (transmitter == null) {
                System.Diagnostics.Debug.WriteLine("TransmitController::Transmit: Transmitter not found.");
				return BadRequest();
			}

			// For each tag that the transmitter is telling us about:
			foreach (var item in details.TagData) {
				// Update its strength:
				StrengthManager.Update(item.TagUuid, id, (int)transmitter.Type, item.Distance);
			}

			return Ok();
		}

		private static double FeetToMeters(double feet) {
			return feet * 0.3048;
		}

		public struct RequestDetails {
			public string TransmitterId;
			public List<Strength> TagData;
		}

		public struct Strength {
			public string TagUuid;
			public double Distance;
		}
	}
}