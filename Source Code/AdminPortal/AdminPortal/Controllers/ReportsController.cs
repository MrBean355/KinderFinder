using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdminPortal.Models;

namespace AdminPortal.Controllers
{
    public class ReportsController : Controller
    {
        private KinderFinderEntities db = new KinderFinderEntities();

        // GET: Reports
        public ActionResult Index()
        {
            var tags = db.Tags.Include(t => t.Patron);
            return View(tags.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
