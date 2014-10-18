using AdminPortal.Code;

using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Transmitters {

    public class OutOfRangeController : ApiController {

        [HttpPost]
        public IHttpActionResult OutOfRange(RequestDetails details) {
            StrengthManager.FlagTag(details.BeaconId, true);
			System.Diagnostics.Debug.WriteLine("[Info] Tag " + details.BeaconId + " marked as out of range!");
            return Ok();
        }

        public struct RequestDetails {
            public string BeaconId;
        }
    }
}