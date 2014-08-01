using AdminPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

    public class UnlinkTagController : ApiController {
        private IKinderFinderContext db = new KinderFinderEntities();

		public UnlinkTagController() { }

		public UnlinkTagController(IKinderFinderContext context) {
			db = context;
		}

        [HttpPost]
        public IHttpActionResult UnlinkTag(RequestDetails details) {
            var patrons = from item in db.Patrons
                          where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
                          select item;

			if (patrons.Count() == 0)
				return BadRequest();

            var patron = patrons.First();

            var tags = from item in db.Tags
                       where item.Label.Equals(details.TagLabel, StringComparison.CurrentCultureIgnoreCase) && item.CurrentPatron == patron.ID
                       select item;

			if (tags.Count() == 0)
				return BadRequest();

            var tag = tags.First();
            tag.CurrentPatron = null;
            db.SaveChanges();

			return Ok();
        }

        public struct RequestDetails {
            public string EmailAddress,
                TagLabel;
        }
    }

    public class LinkTagController : ApiController {
        private IKinderFinderContext db = new KinderFinderEntities();

		public LinkTagController() { }

		public LinkTagController(IKinderFinderContext context) {
			db = context;
		}

        [HttpPost]
        public IHttpActionResult LinkTag(RequestDetails details) {
            var tags = from item in db.Tags
                       where item.Label.Equals(details.TagLabel, StringComparison.CurrentCultureIgnoreCase) && item.CurrentPatron == null
                       select item;

            var patrons = from item in db.Patrons
                          where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
                          select item;

			if (tags.Count() == 0 || patrons.Count() == 0)
				return BadRequest();

            var tag = tags.First();
            var patron = patrons.First();

            tag.CurrentPatron = patron.ID;
            db.SaveChanges();

			return Ok();
        }

        public struct RequestDetails {
            public string EmailAddress,
                TagLabel;
        }
    }

    public class FreeTagListController : ApiController {
        private IKinderFinderContext db = new KinderFinderEntities();

		public FreeTagListController() { }

		public FreeTagListController(IKinderFinderContext context) {
			db = context;
		}

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
        private IKinderFinderContext db = new KinderFinderEntities();

		public TagListController() { }

		public TagListController(IKinderFinderContext context) {
			db = context;
		}

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