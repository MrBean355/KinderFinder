using AdminPortal.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers {

	public class RegisterController : ApiController {
		private IKinderFinderContext db = new KinderFinderEntities();

		public RegisterController(IKinderFinderContext context) {
			db = context;
		}

		/**
		 * Attempts to register a new account for a user.
		 * @param patron New user to register.
		 * @returns OK if successful; Conflict if email already exists;
		 * InternalServerError otherwise.
		 */
		[HttpPost]
		public IHttpActionResult Register(Patron patron) {
			var query = from item in db.Patrons
						where item.EmailAddress.Equals(patron.EmailAddress, System.StringComparison.CurrentCultureIgnoreCase)
						select item;

			/* Make sure email address is not already in use. */
			int count = query.Count(me => me.EmailAddress == patron.EmailAddress);

			if (count > 0)
				return Conflict();

			/* Attempt to insert patron into database. */
			try {
				if (!ModelState.IsValid)
					return InternalServerError();

				db.Patrons.Add(patron);
				db.SaveChanges();
			}
			catch (Exception ex) {
				return InternalServerError();
			}

			/* Nothing went wrong; success! */
			return Ok();
		}
	}
}