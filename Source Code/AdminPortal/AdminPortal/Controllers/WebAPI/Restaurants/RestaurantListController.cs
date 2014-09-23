using AdminPortal.Models;

using System.Collections.Generic;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Restaurants {

	/**
	 * Simply retrieves a list of all restaurants in the system.
	 */
	public class RestaurantListController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetRestaurants() {
			var result = new List<string>();

			foreach (var item in db.Restaurants)
				result.Add(item.Name);

			return Ok(result);
		}
	}
}