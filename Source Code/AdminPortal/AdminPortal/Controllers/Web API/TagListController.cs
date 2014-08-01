using AdminPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

    public class UnlinkTagController : ApiController {
        private KinderFinderEntities db = new KinderFinderEntities();

        [HttpPost]
        public HttpResponseMessage UnlinkTag(RequestDetails details) {
            var patrons = from item in db.Patrons
                          where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
                          select item;

            if (patrons == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var patron = patrons.First();

            var tags = from item in db.Tags
                       where item.Label.Equals(details.TagLabel, StringComparison.CurrentCultureIgnoreCase) && item.CurrentPatron == patron.ID
                       select item;

            if (tags == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var tag = tags.First();
            tag.CurrentPatron = null;
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public struct RequestDetails {
            public string EmailAddress,
                TagLabel;
        }
    }

    public class LinkTagController : ApiController {
        private KinderFinderEntities db = new KinderFinderEntities();

        [HttpPost]
        public HttpResponseMessage LinkTag(RequestDetails details) {
            var tags = from item in db.Tags
                       where item.Label.Equals(details.TagLabel, StringComparison.CurrentCultureIgnoreCase) && item.CurrentPatron == null
                       select item;

            var patrons = from item in db.Patrons
                          where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
                          select item;

            if (tags == null || patrons == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var tag = tags.First();
            var patron = patrons.First();

            tag.CurrentPatron = patron.ID;
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public struct RequestDetails {
            public string EmailAddress,
                TagLabel;
        }
    }

    public class FreeTagListController : ApiController {
        private KinderFinderEntities db = new KinderFinderEntities();

        /**
         * Returns a list of all unused tags.
         */
        [HttpPost]
        public IEnumerable<string> GetFreeTags() {
            var tags = from item in db.Tags
                       where item.CurrentPatron == null
                       select item.Label;

            return new List<string>(tags);
        }
    }

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
            var ids = from pat in db.Patrons
                      where pat.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
                      select pat.ID;

            /* If user actually exists. */
            if (ids.Count() > 0) {
                var id = ids.First();

                /* Find all linked tags and add to list. */
                var query = from tag in db.Tags
                            where tag.CurrentPatron == id
                            select tag.Label;

                foreach (var label in query)
                    result.Add(label);
            }

            return result;
        }

        public struct RequestDetails {
            public string EmailAddress;
        }
    }
}