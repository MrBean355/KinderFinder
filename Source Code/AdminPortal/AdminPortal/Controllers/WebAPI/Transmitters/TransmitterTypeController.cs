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

			var types = new List<string>();
			types.Add("1");
			types.Add("2");
			types.Add("3");

			var trans = from item in Db.Transmitters
						where item.Restaurant == restaurant.ID
						select item;

			foreach (var item in trans) {
				string type = (int)item.Type + "";

				if (types.Contains(type))
					types.Remove(type);
			}

			if (types.Count == 0)
				return Conflict();

			return Ok(types);
		}

		public struct RequestDetails {
			public string RestaurantName;
		}
	}
}