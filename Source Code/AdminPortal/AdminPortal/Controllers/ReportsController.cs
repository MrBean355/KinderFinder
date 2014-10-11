﻿using AdminPortal.Models;

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
        public ActionResult AppUsersReport()
        {
            return View(db.AppUsers.ToList());
        }

        // GET: TagssReport
        public ActionResult TagsReport(int? SelectedRestaurant)
        {
            var restaurants = db.Restaurants.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedRestaurant = new SelectList(restaurants, "ID", "Name", SelectedRestaurant);
            int restaurantID = SelectedRestaurant.GetValueOrDefault();

            IQueryable<Tag> tags = db.Tags
                .Where(t => !SelectedRestaurant.HasValue || t.Restaurant == restaurantID)
                .OrderBy(r => r.ID);
            var sql = tags.ToString();
            return View(tags.ToList());
        }

		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
