using AdminPortal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

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

	public class LinkRestaurantController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult PickRestaurant(RequestDetails details) {
			var user = (from item in db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
						select item).FirstOrDefault();

			var restaurant = (from item in db.Restaurants
						where item.Name.Equals(details.Restaurant)
						select item).FirstOrDefault();

			if (user == null || restaurant == null)
				return BadRequest();

			user.CurrentRestaurant = restaurant.ID;
			db.SaveChanges();

			return Ok();
		}

		public struct RequestDetails {
			public string EmailAddress,
				Restaurant;
		}
	}
}