using AdminPortal.Models;

using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdminPortal.Controllers {

	[Authorize]
	public class AdminsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		// GET: Admins
		public ActionResult Index() {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			return View(db.AspNetUsers.ToList());
		}

		// POST: Admins/Details/5
		public ActionResult Details(string id) {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (id.Equals(""))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			AspNetUser user = db.AspNetUsers.Find(id);

			if (user == null)
				return HttpNotFound();

			return View(user);
		}

		// GET: Admins/Create
		public ActionResult Create() {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			return View();
		}

		// POST: Admins/Create
		[HttpPost]
		public ActionResult Create(AspNetUser user) {
			return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		}

		// GET: Admins/Edit/5
		public ActionResult Edit(string id) {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (id.Equals(""))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			AspNetUser user = db.AspNetUsers.Find(id);

			if (user == null)
				return HttpNotFound();

			return View(user);
		}

		// POST: Admins/Edit/5
		[HttpPost]
		public ActionResult Edit(AspNetUser user) {
			if (ModelState.IsValid) {
				db.Entry(user).State = System.Data.Entity.EntityState.Modified;
				db.SaveChanges();

				return RedirectToAction("Index");
			}

			return View(user);
		}

		// GET: Admins/Delete/5
		public ActionResult Delete(string id) {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (id.Equals(""))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			AspNetUser user = db.AspNetUsers.Find(id);

			if (user == null)
				return HttpNotFound();

			return View(user);
		}

		// POST: Admins/Delete/5
		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection) {
			try {
				// TODO: Add delete logic here

				return RedirectToAction("Index");
			}
			catch {
				return View();
			}
		}
	}
}
