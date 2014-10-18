using AdminPortal.Models;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdminPortal.Controllers {

	[Authorize]
	public class AppUsersController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		/// <summary>
		/// Determines whether the app user with specified ID is accessible by
		/// the currently logged in user.
		/// </summary>
		/// <param name="id">ID of the app user.</param>
		/// <returns>True if accessible; false otherwise.</returns>
		private bool IsUserAccessible(int id) {
			if (!User.IsInRole("GlobalAdmins")) {
				var query = (from admin in db.AspNetUsers
							join rest in db.Restaurants on admin.Id equals rest.Admin
							join user in db.AppUsers on rest.ID equals user.CurrentRestaurant
							where admin.UserName.Equals(User.Identity.Name) && user.ID == id
							select user).FirstOrDefault();

				return query != null;
			}

			return true;
		}

		// GET: AppUsers
		public ActionResult Index() {
			if (User.IsInRole("GlobalAdmins"))
				return View(db.AppUsers.ToList());

			var result = new List<AppUser>();

			// Gets all app users that are currently at the admin's current
			// restaurants.
			var query = from admin in db.AspNetUsers
						join rest in db.Restaurants on admin.Id equals rest.Admin
						join user in db.AppUsers on rest.ID equals user.CurrentRestaurant
						where admin.UserName.Equals(User.Identity.Name)
						select user;

			if (query != null)
				result.AddRange(query);

			return View(result);
		}

		// GET: AppUsers/Details/5
		public ActionResult Details(int? id) {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			if (!IsUserAccessible((int)id))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			AppUser appUser = db.AppUsers.Find(id);

			if (appUser == null)
				return HttpNotFound();

			return View(appUser);
		}

		// GET: AppUsers/Create
		/*public ActionResult Create() {
			ViewBag.CurrentRestaurant = new SelectList(db.Restaurants, "ID", "Name");
			return View();
		}*/

		// POST: AppUsers/Create
		/*[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,FirstName,Surname,EmailAddress,PhoneNumber,PasswordHash,CurrentRestaurant")] AppUser appUser) {
			if (ModelState.IsValid) {
				db.AppUsers.Add(appUser);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(appUser);
		}*/

		// GET: AppUsers/Edit/5
		/*public ActionResult Edit(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AppUser appUser = db.AppUsers.Find(id);
			if (appUser == null) {
				return HttpNotFound();
			}
			ViewBag.CurrentRestaurant = new SelectList(db.Restaurants, "ID", "Name", appUser.CurrentRestaurant);
			return View(appUser);
		}*/

		// POST: AppUsers/Edit/5
		/*[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,FirstName,Surname,EmailAddress,PhoneNumber,PasswordHash,CurrentRestaurant")] AppUser appUser) {
			if (ModelState.IsValid) {
				db.Entry(appUser).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(appUser);
		}*/

		// GET: AppUsers/Delete/5
		/*public ActionResult Delete(int? id) {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			if (!IsUserAccessible((int)id))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			AppUser appUser = db.AppUsers.Find(id);

			if (appUser == null)
				return HttpNotFound();

			return View(appUser);
		}

		// POST: AppUsers/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id) {
			AppUser appUser = db.AppUsers.Find(id);
			db.AppUsers.Remove(appUser);
			db.SaveChanges();
			return RedirectToAction("Index");
		}*/

		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
