using AdminPortal.Models;

using System;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	public class LoginController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		/**
		 * Attempts to log a user in.
		 * @param details User's entered details.
		 * @returns OK if successful; BadRequest otherwise.
		 */
		[HttpPost]
		public IHttpActionResult Login(LoginDetails details) {
			/* Find first user with matching email address. */
			var password = (from item in db.AppUsers
							where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
							select item.PasswordHash).FirstOrDefault();

			/* Check whether passwords match. */
			if (password != null && password.Equals(details.PasswordHash))
				return Ok();

			return BadRequest();
		}

		public struct LoginDetails {
			public string EmailAddress, PasswordHash;
		}
	}
}