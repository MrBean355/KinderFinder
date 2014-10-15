using AdminPortal.Models;

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdminPortal.Controllers {
	
	[Authorize] // This means that you have to be logged in to the site to be able to access
				// any of the reports actions. Otherwise you can go to localhost/reports and
				// still get access if you aren't logged in.
	public class ReportsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		// GET: Reports
		public ActionResult Index() {
			return View();
		}

        // GET: AppUsersReport
        public ActionResult AppUsersReport(int? SelectedRestaurant = null)
        {
            IQueryable<AppUserStat> appUsers;
            int restaurantID = SelectedRestaurant.GetValueOrDefault();
            if (User.IsInRole("GlobalAdmins"))
            {
                var restaurants = db.Restaurants.OrderBy(q => q.Name).ToList();
                ViewBag.SelectedRestaurant = new SelectList(restaurants, "ID", "Name", SelectedRestaurant);
                restaurantID = SelectedRestaurant.GetValueOrDefault();
                appUsers = db.AppUserStats
                    .Where(t => !SelectedRestaurant.HasValue || t.LastRestaurant == restaurantID)
                    .OrderBy(r => r.VisitCount);
                ViewBag.DropdownListOptionLabel = "All";
            }
            else
            {
                // Fetches all tags that belong to the current admin.
                // Basically gets all tags owned by restaurants that the user is an admin of.
                var restaurants = from user in db.AspNetUsers
                                  join rest in db.Restaurants
                                      on user.Id equals rest.Admin
                                  where user.UserName.Equals(User.Identity.Name)
                                  orderby rest.Name
                                  select rest;

                ViewBag.SelectedRestaurant = new SelectList(restaurants, "ID", "Name", SelectedRestaurant);
                restaurantID = SelectedRestaurant.GetValueOrDefault();
                appUsers = db.AppUserStats
                    .Where(t => SelectedRestaurant.HasValue || t.LastRestaurant == restaurantID)
                    .OrderBy(r => r.VisitCount);
                ViewBag.DropdownListOptionLabel = System.String.Empty;
            }

            return View(appUsers.ToList());
        }

        // GET: TagsReport
        public ActionResult TagsReport(int? SelectedRestaurant = null)
        {
            IQueryable<Tag> tags;
            int restaurantID = SelectedRestaurant.GetValueOrDefault();
            if (User.IsInRole("GlobalAdmins"))
            {
                var restaurants = db.Restaurants.OrderBy(q => q.Name).ToList();
                ViewBag.SelectedRestaurant = new SelectList(restaurants, "ID", "Name", SelectedRestaurant);
                restaurantID = SelectedRestaurant.GetValueOrDefault();
                tags = db.Tags
                    .Where(t => !SelectedRestaurant.HasValue || t.Restaurant == restaurantID)
                    .OrderBy(r => r.ID);
                ViewBag.DropdownListOptionLabel = "All";
            }
            else
            {
                // Fetches all tags that belong to the current admin.
                // Basically gets all tags owned by restaurants that the user is an admin of.
                var restaurants = from user in db.AspNetUsers
                                  join rest in db.Restaurants
                                      on user.Id equals rest.Admin
                                      where user.UserName.Equals(User.Identity.Name)
                                      orderby rest.Name
                                      select rest;           

                ViewBag.SelectedRestaurant = new SelectList(restaurants, "ID", "Name", SelectedRestaurant);
                restaurantID = SelectedRestaurant.GetValueOrDefault();
                tags = db.Tags
                    .Where(t => SelectedRestaurant.HasValue || t.Restaurant == restaurantID)
                    .OrderBy(r => r.ID);
                ViewBag.DropdownListOptionLabel = System.String.Empty;
            }

            return View(tags.ToList());
        }

        // GET: ZonesReport
        public ActionResult ZonesReport(int? SelectedRestaurant = null)
        {
            IQueryable<Zone> zones;
            int restaurantID = SelectedRestaurant.GetValueOrDefault();
            if (User.IsInRole("GlobalAdmins"))
            {
                var restaurants = db.Restaurants.OrderBy(q => q.Name).ToList();
                ViewBag.SelectedRestaurant = new SelectList(restaurants, "ID", "Name", SelectedRestaurant);
                restaurantID = SelectedRestaurant.GetValueOrDefault();
                zones = db.Zones
                    .Where(t => !SelectedRestaurant.HasValue || t.Restaurant == restaurantID)
                    .OrderBy(r => r.ID);
                ViewBag.DropdownListOptionLabel = "All";
            }
            else
            {
                // Fetches all tags that belong to the current admin.
                // Basically gets all tags owned by restaurants that the user is an admin of.
                var restaurants = from user in db.AspNetUsers
                                  join rest in db.Restaurants
                                      on user.Id equals rest.Admin
                                  where user.UserName.Equals(User.Identity.Name)
                                  orderby rest.Name
                                  select rest;

                ViewBag.SelectedRestaurant = new SelectList(restaurants, "ID", "Name", SelectedRestaurant);
                restaurantID = SelectedRestaurant.GetValueOrDefault();
                zones = db.Zones
                    .Where(t => SelectedRestaurant.HasValue || t.Restaurant == restaurantID)
                    .OrderBy(r => r.ID);
                ViewBag.DropdownListOptionLabel = System.String.Empty;
            }

            return View(zones.ToList());
        }

		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
