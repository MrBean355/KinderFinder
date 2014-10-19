using AdminPortal.Models;

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Transmitters {
	
	/**
	 * Retrieves a list of available transmitter types for a restaurant.
	 */
	public class TransmitterTypeController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetAvailableTypes(RequestDetails details) {
			var restaurant = (from item in Db.Restaurants
							  where item.Name.Equals(details.RestaurantName)
							  select item).FirstOrDefault();

			if (restaurant == null)
				return BadRequest();

			var availableTypes = new List<string>();
			availableTypes.Add("1");
			availableTypes.Add("2");
			availableTypes.Add("3");

			var transmitters = from item in Db.Transmitters
						where item.Restaurant == restaurant.ID
						select item;

			foreach (var item in transmitters) {
				string type = (int)item.Type + "";

				if (availableTypes.Contains(type))
					availableTypes.Remove(type);
			}

			if (availableTypes.Count == 0)
				return Conflict();

			return Ok(availableTypes);
		}

		public struct RequestDetails {
			public string RestaurantName;
		}
	}
}