using AdminPortal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Tags {

	/**
	 * Retrieves a list of tags that are linked to a particular user.
	 */
	public class TagListController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetTags(RequestDetails details) {
			/* Find patron's ID and current restaurant. */
			var user = (from item in db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
						select new { item.ID, item.CurrentRestaurant }).FirstOrDefault();

			if (user == null)
				return BadRequest();

			var result = new List<string>();

			/* Find all linked tags and add to list. */
			var query = from tag in db.Tags
						where tag.CurrentUser == user.ID && tag.Restaurant == user.CurrentRestaurant
						select tag.Label;

			foreach (var label in query)
				result.Add(label);

			return Ok(result);
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}