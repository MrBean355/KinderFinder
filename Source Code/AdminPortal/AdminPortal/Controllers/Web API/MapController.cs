using AdminPortal.Models;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

    public class MapController : ApiController {
        private KinderFinderEntities db = new KinderFinderEntities();

        [HttpPost]
        public HttpResponseMessage GetCurrentMap() {
            var maps = from item in db.Maps
                       where item.Active
                       select item.Data;

            if (maps == null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            byte[] data = maps.First();

            MemoryStream m = new MemoryStream(data);
            Image img = Image.FromStream(m);
            img.Save(@"C:\test.jpg");

            MemoryStream ms = new MemoryStream(data);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

            return response;
        }
    }
}