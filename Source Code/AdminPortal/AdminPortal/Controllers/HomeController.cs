using System.Web.Mvc;

namespace AdminPortal.Controllers {
	public class HomeController : Controller {
		public ActionResult Index() {
			return View();
		}

		public ActionResult About() {
			ViewBag.Message = "";

			return View();
		}

		public ActionResult Contact() {
			return View();
		}
	}
}