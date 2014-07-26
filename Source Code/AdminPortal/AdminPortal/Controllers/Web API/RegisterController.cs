using AdminPortal.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers {

	public class RegisterController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		/**
		 * Attempts to register a new account for a user.
		 * @param patron New user to register.
		 * @returns OK if successful; Conflict if email already exists;
		 * InternalServerError otherwise.
		 */
		[HttpPost]
		public HttpResponseMessage Register(Patron patron) {
			var query = from item in db.Patrons
						where item.EmailAddress.Equals(patron.EmailAddress, System.StringComparison.CurrentCultureIgnoreCase)
						select item;

			/* Make sure email address is not already in use. */
			int count = query.Count(me => me.EmailAddress == patron.EmailAddress);

			if (count > 0)
				return Request.CreateResponse(HttpStatusCode.Conflict);

			/* Attempt to insert patron into database. */
			try {
				if (!ModelState.IsValid)
					return Request.CreateResponse(HttpStatusCode.InternalServerError, "Invalid model state.");

				db.Patrons.Add(patron);
				db.SaveChanges();
			}
			catch (Exception ex) {
				return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown server error.");
			}

			/* Nothing went wrong; success! */
			return Request.CreateResponse(HttpStatusCode.OK);
		}
	}
}