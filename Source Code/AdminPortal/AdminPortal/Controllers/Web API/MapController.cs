using AdminPortal.Models;

using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

    public class MapController : ApiController {
        private KinderFinderEntities db = new KinderFinderEntities();

        [HttpPost]
        public HttpResponseMessage GetCurrentMap() {
            var mapData = (from item in db.Maps
                       where item.Active
                       select item.Data).FirstOrDefault();

            if (mapData == null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

			response.Content = new StreamContent(new MemoryStream(mapData));
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

            return response;
        }
    }

	public class MapSizeController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetCurrentMapSize() {
			var mapData = (from item in db.Maps
						   where item.Active
						   select item.Data).FirstOrDefault();

			if (mapData == null)
				return BadRequest();

			return Ok(mapData.Length);
		}
	}
}