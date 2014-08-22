﻿using AdminPortal.Models;

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
			/* Find all users with matching email (should only be 1). */
			var query = from item in db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
						select item.PasswordHash;

			/* Check whether passwords match. */
			foreach (string password in query) {
				if (password.Equals(details.PasswordHash))
					return Ok();
			}
			
			/* Invalid login details:
			 * - Email is wrong or doesn't exist.
			 * - Password is wrong.
			 */
			return BadRequest();
		}

		public struct LoginDetails {
			public string EmailAddress, PasswordHash;
		}
	}
}