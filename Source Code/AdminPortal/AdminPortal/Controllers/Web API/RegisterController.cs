using AdminPortal.Models;

using System;
using System.Linq;
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
		public IHttpActionResult Register(AppUser user) {
			var query = from item in db.AppUsers
						where item.EmailAddress.Equals(user.EmailAddress, System.StringComparison.CurrentCultureIgnoreCase)
						select item;
			
			/* Make sure email address is not already in use. */
			int count = query.Count(me => me.EmailAddress == user.EmailAddress);

			if (count > 0)
				return Conflict();

			/* Attempt to insert patron into database. */
			try {
				if (!ModelState.IsValid)
					return InternalServerError();

				db.AppUsers.Add(user);
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