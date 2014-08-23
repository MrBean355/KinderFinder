using AdminPortal.Models;

using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AdminPortal.Controllers {

	[Authorize]
	public class RestaurantsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		// Used to retrieve stored images.
		public FileContentResult GetMap(int? id) {
			if (!User.IsInRole("GlobalAdmins") || id == null)
				return null;

			Restaurant restaurant = db.Restaurants.Find(id);

			if (restaurant == null || restaurant.Map == null)
				return null;

			return new FileContentResult(restaurant.Map, "image/jpeg");
		}

		// GET: Restaurants
		public ActionResult Index() {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			return View(db.Restaurants.ToList());
		}

		// GET: Restaurants/Details/5
		public ActionResult Details(int? id) {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Restaurant restaurant = db.Restaurants.Find(id);
			if (restaurant == null) {
				return HttpNotFound();
			}
			return View(restaurant);
		}

		// GET: Restaurants/Create
		public ActionResult Create() {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			ViewBag.Admin = new SelectList(db.AspNetUsers, "Id", "Email");

			return View();
		}

		// POST: Restaurants/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Name,Map,Admin")] Restaurant restaurant, HttpPostedFileBase file) {
			if (ModelState.IsValid) {
				if (file != null) {
					MemoryStream target = new MemoryStream();
					file.InputStream.CopyTo(target);
					restaurant.Map = target.ToArray();
				}

				db.Restaurants.Add(restaurant);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(restaurant);
		}

		// GET: Restaurants/Edit/5
		public ActionResult Edit(int? id) {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Restaurant restaurant = db.Restaurants.Find(id);
			if (restaurant == null) {
				return HttpNotFound();
			}
			ViewBag.Admin = new SelectList(db.AspNetUsers, "Id", "Email", restaurant.Admin);
			return View(restaurant);
		}

		// POST: Restaurants/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Name,Map,Admin")] Restaurant restaurant, HttpPostedFileBase file) {
			if (ModelState.IsValid) {
				if (file != null) {
					MemoryStream target = new MemoryStream();
					file.InputStream.CopyTo(target);
					restaurant.Map = target.ToArray();
				}

				db.Entry(restaurant).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(restaurant);
		}

		// GET: Restaurants/Delete/5
		public ActionResult Delete(int? id) {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Restaurant restaurant = db.Restaurants.Find(id);
			if (restaurant == null) {
				return HttpNotFound();
			}
			return View(restaurant);
		}

		// POST: Restaurants/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int? id) {
			Restaurant restaurant = db.Restaurants.Find(id);
			db.Restaurants.Remove(restaurant);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
