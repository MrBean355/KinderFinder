using AdminPortal.Models;

using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.Accounts {

	/**
	 * Registers a new account for an app user.
	 */
	public class RegisterController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult Register(AppUser newUser) {
			var user = (from item in db.AppUsers
						where item.EmailAddress.Equals(newUser.EmailAddress, System.StringComparison.CurrentCultureIgnoreCase)
						select item).FirstOrDefault();

			/* Make sure email address is not already in use. */
			if (user == null)
				return Conflict();

			/* Insert patron into database. */
			db.AppUsers.Add(newUser);
			db.SaveChanges();

			return Ok();
		}
	}
}