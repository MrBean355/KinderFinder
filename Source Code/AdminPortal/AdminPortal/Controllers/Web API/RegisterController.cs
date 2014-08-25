using AdminPortal.Models;

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
		public IHttpActionResult Register(AppUser newUser) {
			var user = (from item in db.AppUsers
						where item.EmailAddress.Equals(newUser.EmailAddress, System.StringComparison.CurrentCultureIgnoreCase)
						select item).FirstOrDefault();

			/* Make sure email address is not already in use. */
			//int count = query.Count(me => me.EmailAddress == user.EmailAddress);

			if (user == null)
				return Conflict();

			/* Insert patron into database. */
			db.AppUsers.Add(newUser);
			db.SaveChanges();

			return Ok();
		}
	}
}