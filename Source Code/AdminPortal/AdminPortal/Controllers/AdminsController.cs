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
				return RedirectToAction("Index", "Home");

			return View(db.AspNetUsers.ToList());
		}

		// POST: Admins/Details/5
		public ActionResult Details(string id) {
			if (!User.IsInRole("GlobalAdmins"))
				return RedirectToAction("Index", "Home");

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
				return RedirectToAction("Index", "Home");

			return View();
		}

		// POST: Admins/Create
		[HttpPost]
		public ActionResult Create(AspNetUser user) {
			if (!User.IsInRole("GlobalAdmins"))
				return RedirectToAction("Index", "Home");

			return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		}

		// GET: Admins/Edit/5
		public ActionResult Edit(string id) {
			if (!User.IsInRole("GlobalAdmins"))
				return RedirectToAction("Index", "Home");

			if (id.Equals(""))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			AspNetUser user = db.AspNetUsers.Find(id);

			if (user == null)
				return HttpNotFound();

			return View(user);
		}

		// POST: Admins/Edit/5
		[HttpPost]
		public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser user) {
			if (!User.IsInRole("GlobalAdmins"))
				return RedirectToAction("Index", "Home");

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
				return RedirectToAction("Index", "Home");

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
			if (!User.IsInRole("GlobalAdmins"))
				return RedirectToAction("Index", "Home");

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
