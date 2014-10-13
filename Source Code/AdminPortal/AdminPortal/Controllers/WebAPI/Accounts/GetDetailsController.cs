using AdminPortal.Models;

using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Accounts {

	public class GetDetailsController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetDetails(RequestDetails details) {
			var user = (from item in Db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress)
						select item).FirstOrDefault();

			if (user == null)
				return BadRequest();

			Response result = new Response();
			result.FirstName = user.FirstName;
			result.Surname = user.Surname;
			result.PhoneNumber = user.PhoneNumber;

			return Ok(result);
		}

		public struct Response {
			public string FirstName,
				Surname,
				PhoneNumber;
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}