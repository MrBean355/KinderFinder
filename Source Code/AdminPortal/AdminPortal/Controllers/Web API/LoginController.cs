using AdminPortal.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

	public class LoginController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public HttpResponseMessage Login(LoginDetails details) {
			var query = from item in db.Patrons
						where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
						select item.PasswordHash;

			foreach (string password in query) {
				if (password.Equals(details.PasswordHash))
					return Request.CreateResponse(HttpStatusCode.OK);
			}

			return Request.CreateResponse(HttpStatusCode.BadRequest);
		}

		public struct LoginDetails {
			public string EmailAddress, PasswordHash;
		}
	}
}