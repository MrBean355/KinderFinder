using AdminPortal.Models;

using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Maps {

	/**
	 * Retrieves the number of bytes that the current map for a restaurant is
	 * in size.
	 */
	public class MapSizeController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetCurrentMapSize(RequestDetails details) {
			// Load the map of the restaurant the user is currently at:
			var mapData = (from user in db.AppUsers
						   join rest in db.Restaurants on user.CurrentRestaurant equals rest.ID
						   where user.EmailAddress.Equals(details.EmailAddress)
						   select rest.Map).FirstOrDefault();

			if (mapData == null)
				return BadRequest();

			return Ok(mapData.Length);
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}