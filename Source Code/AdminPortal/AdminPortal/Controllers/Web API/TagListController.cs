using AdminPortal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	public class TagListController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		/**
		 * Returns a list of all tags associated with a user.
		 * @param details User's details.
		 * @returns List of associated tags.
		 */
		[HttpPost]
		public IEnumerable<Tag> GetTags(RequestDetails details) {
			var r = new List<Tag>(db.Tags);

			Console.WriteLine(JsonConvert.SerializeObject(r));
			return null;

			/* Find patron's ID. *
			var ids = from pat in db.Patrons
					  where pat.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
					  select pat.ID;

			/* If user actually exists. *
			if (ids.Count() > 0) {
				var id = ids.First();

				/* Find all linked tags and add to list. *
				var query = from tag in db.Tags
							where tag.CurrentPatron == id
							select tag.Label;

				foreach (var label in query)
					result.Add(label);
			}

			return result;*/
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}