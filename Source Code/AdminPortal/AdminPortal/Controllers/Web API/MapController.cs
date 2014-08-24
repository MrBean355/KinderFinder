using AdminPortal.Models;

using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	/// <summary>
	/// Returns the number of bytes that an app user's current restaurant's map
	/// takes up.
	/// </summary>
	public class MapSizeController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetCurrentMapSize(RequestDetails details) {
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

	/// <summary>
	/// Returns the actual bytes making up an app user's current restaurant's
	/// map.
	/// </summary>
	public class MapController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public HttpResponseMessage GetCurrentMap(RequestDetails details) {
			var mapData = (from user in db.AppUsers
						   join rest in db.Restaurants on user.CurrentRestaurant equals rest.ID
						   where user.EmailAddress.Equals(details.EmailAddress)
						   select rest.Map).FirstOrDefault();

			if (mapData == null)
				return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

			response.Content = new StreamContent(new MemoryStream(mapData));
			response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

			return response;
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}