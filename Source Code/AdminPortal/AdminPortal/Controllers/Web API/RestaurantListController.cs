using AdminPortal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	public class RestaurantListController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IEnumerable<string> GetRestaurants() {
			var result = new List<string>();

			foreach (var item in db.Restaurants)
				result.Add(item.Name);

			return result;
		}
	}

	public class LinkRestaurantController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult PickRestaurant(RequestDetails details) {
			var result = new List<string>();

			var user = (from item in db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
						select item).FirstOrDefault();

			var rest = (from item in db.Restaurants
						where item.Name.Equals(details.Restaurant)
						select item).FirstOrDefault();

			if (user != null && rest != null) {
				user.CurrentRestaurant = rest.ID;
				db.SaveChanges();

				return Ok();
			}

			return BadRequest();
		}

		public struct RequestDetails {
			public string EmailAddress,
				Restaurant;
		}
	}
}