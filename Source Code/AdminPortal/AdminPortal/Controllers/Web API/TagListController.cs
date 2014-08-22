using AdminPortal.Models;

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
        public IEnumerable<string> GetTags(RequestDetails details) {
            var result = new List<string>();

            /* Find patron's ID. */
			var user = (from item in db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
						select item).First();

            /* Find all linked tags and add to list. */
            var query = from tag in db.Tags
                        where tag.CurrentUser == user.ID && tag.Restaurant == user.CurrentRestaurant
                        select tag.Label;

            foreach (var label in query)
                result.Add(label);

            return result;
        }

        public struct RequestDetails {
            public string EmailAddress;
        }
    }
}