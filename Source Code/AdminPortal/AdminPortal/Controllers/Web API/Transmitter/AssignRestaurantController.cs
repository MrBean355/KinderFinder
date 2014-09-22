using AdminPortal.Models;

using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	public class AssignRestaurantController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();
		
		[HttpPost]
		public IHttpActionResult AssignRestaurant(RequestDetails details) {
			
			/*System.Diagnostics.Debug.WriteLine("Attempting to assign");

			if (details.ID == null || details.RestaurantName == null) {
				return BadRequest();
			}

			var restaurant = (from item in Db.Restaurants
							 where item.Name.Equals(details.RestaurantName)
							 select item).FirstOrDefault();

			if (restaurant == null) {
				return BadRequest();
			}

			System.Diagnostics.Debug.WriteLine("\tSomeone linked to " + restaurant.Name);

			return Ok();*/
		}

		public struct RequestDetails {
			public string RestaurantName;
		}
	}
}