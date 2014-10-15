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
    [Authorize]
    public class ZonesController : Controller
    {
        private KinderFinderEntities db = new KinderFinderEntities();

        // GET: Zones
        public ActionResult Index()
        {
            var zones = db.Zones.Include(z => z.RestaurantZ).Include(z => z.ZonePa);
            return View(zones.ToList());
        }

        // GET: Zones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zone zone = db.Zones.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            return View(zone);
        }

        // GET: Zones/Create
        public ActionResult Create()
        {
            ViewBag.Restaurant = new SelectList(db.Restaurants, "ID", "Name");
            ViewBag.PrecedingZone = new SelectList(db.Zones, "ID", "ZoneName");
            return View();
        }

        // POST: Zones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Zone zone)
        {
            if (ModelState.IsValid)
            {
                db.Zones.Add(zone);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Restaurant = new SelectList(db.Restaurants, "ID", "Name", zone.Restaurant);
            ViewBag.PrecedingZone = new SelectList(db.Zones, "ID", "ZoneName", null);
            return View(zone);
        }

        // GET: Zones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zone zone = db.Zones.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            ViewBag.Restaurant = new SelectList(db.Restaurants, "ID", "Name", zone.Restaurant);

            var ZoneQuery = from z in db.Zones
                            orderby z.ZoneName
                            where z.ID != id
                            select z;
            ViewBag.PrecedingZone = new SelectList(ZoneQuery, "ID", "ZoneName", zone.PrecedingZone);
            return View(zone);
        }

        // POST: Zones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ZoneName,Restaurant,Status,PrecedingZone")] Zone zone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Restaurant = new SelectList(db.Restaurants, "ID", "Name", zone.Restaurant);
            ViewBag.PrecedingZone = new SelectList(db.Zones, "ID", "ZoneName", zone.PrecedingZone);
            return View(zone);
        }

        // GET: Zones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zone zone = db.Zones.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            return View(zone);
        }

        // POST: Zones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Zone zone = db.Zones.Find(id);
            db.Zones.Remove(zone);
            db.SaveChanges();
            return RedirectToAction("Index");
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
