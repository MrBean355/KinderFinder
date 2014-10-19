using AdminPortal.Models;

using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Accounts {

	/**
	 * Allows the user to update their account details from their app.
	 */
	public class EditDetailsController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult EditDetails(RequestDetails details) {
			var user = (from item in Db.AppUsers
						where item.EmailAddress.Equals(details.OldEmailAddress)
						select item).FirstOrDefault();

			if (user == null)
				return BadRequest();

			user.FirstName = details.FirstName;
			user.Surname = details.Surname;
			user.EmailAddress = details.EmailAddress;
			user.PhoneNumber = details.PhoneNumber;

			// If they specified a new password, update it:
			if (!details.PasswordHash.Equals(""))
				user.PasswordHash = details.PasswordHash;

			Db.SaveChanges();

			return Ok();
		}

		public struct RequestDetails {
			public string FirstName,
				Surname,
				OldEmailAddress,
				EmailAddress,
				PhoneNumber,
				PasswordHash;
		}
	}
}