﻿using AdminPortal.Models;

using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Maps {

	/**
	 * Retrives the current map's bytes for a restaurant.
	 */
	public class MapController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public HttpResponseMessage GetCurrentMap(RequestDetails details) {
			// Load the map of the restaurant the user is currently at:
			var mapData = (from user in db.AppUsers
						   join rest in db.Restaurants on user.CurrentRestaurant equals rest.ID
						   where user.EmailAddress.Equals(details.EmailAddress)
						   select rest.Map).FirstOrDefault();

			if (mapData == null)
				return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

			response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
			response.Content = new StreamContent(new MemoryStream(mapData));

			return response;
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}