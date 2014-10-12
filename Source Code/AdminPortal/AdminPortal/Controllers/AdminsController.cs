using AdminPortal.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdminPortal.Controllers {

	[Authorize]
	public class AdminsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		// GET: Admins
        public ActionResult Index(string id, int? restaurantID)
        {
			if (!User.IsInRole("GlobalAdmins"))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var viewModel = new AdminIndexData();

            viewModel.AspNetUsers = db.AspNetUsers
                .OrderBy(i => i.UserName);

            if (id != null)
            {
                ViewBag.AspNetUserID = id.ToString();
                viewModel.Restaurants = viewModel.AspNetUsers.Where(
                    i => i.Id == id).Single().Restaurants1;
            }

            return View(viewModel);
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
            PopulateAssignedRestaurantData(user);

			if (user == null)
				return HttpNotFound();

			return View(user);
		}

        private void PopulateAssignedRestaurantData(AspNetUser user)
        {
            var allRestaurants = db.Restaurants;
            var userRestaurants = new HashSet<int>(user.Restaurants1.Select(c => c.ID));
            var viewModel = new List<AssignedRestaurantData>();
            foreach (var rest in allRestaurants)
            {
                viewModel.Add(new AssignedRestaurantData
                {
                    RestaurantID = rest.ID,
                    Name = rest.Name,
                    Assigned = userRestaurants.Contains(rest.ID)
                });
            }
            ViewBag.Restaurants = viewModel;
        }

		// POST: Admins/Edit/5
		[HttpPost]
		public ActionResult Edit(int? id, string[] selectedRestaurants) {
            var userToUpdate = db.AspNetUsers
                .Find(id);

			if (ModelState.IsValid) {
                try
                {
                    UpdateAspNetUserRestaurants(selectedRestaurants, userToUpdate);
                    db.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
			}

            PopulateAssignedRestaurantData(userToUpdate);
			return View(userToUpdate);
		}

        private void UpdateAspNetUserRestaurants(string[] selectedRestaurants, AspNetUser userToUpdate)
        {
            if (selectedRestaurants == null)
            {
                userToUpdate.Restaurants1 = new List<Restaurant>();
                return;
            }

            var selectedRestaurantsHS = new HashSet<string>(selectedRestaurants);
            var aspNetUserRestaurants = new HashSet<int>
                (userToUpdate.Restaurants1.Select(c => c.ID));
            foreach (var rest in db.Restaurants)
            {
                if (selectedRestaurantsHS.Contains(rest.ID.ToString()))
                {
                    if (!aspNetUserRestaurants.Contains(rest.ID))
                    {
                        userToUpdate.Restaurants1.Add(rest);
                    }
                }
                else
                {
                    if (aspNetUserRestaurants.Contains(rest.ID))
                    {
                        userToUpdate.Restaurants1.Remove(rest);
                    }
                }
            }
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
