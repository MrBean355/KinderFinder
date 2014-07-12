using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdminPortal.Models;

namespace AdminPortal.Controllers {
	[Authorize] // Must be logged in to access any of these pages.
	public class PatronsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		// GET: Patrons
		public ActionResult Index() {
			return View(db.Patrons.ToList());
		}

		// GET: Patrons/Details/5
		public ActionResult Details(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Patron patron = db.Patrons.Find(id);
			if (patron == null) {
				return HttpNotFound();
			}
			return View(patron);
		}

		// GET: Patrons/Create
		public ActionResult Create() {
			return View();
		}

		// POST: Patrons/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,FirstName,Surname,PasswordHash,EmailAddress")] Patron patron) {
			if (ModelState.IsValid) {
				db.Patrons.Add(patron);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(patron);
		}

		// GET: Patrons/Edit/5
		public ActionResult Edit(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Patron patron = db.Patrons.Find(id);
			if (patron == null) {
				return HttpNotFound();
			}
			return View(patron);
		}

		// POST: Patrons/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,FirstName,Surname,PasswordHash,EmailAddress")] Patron patron) {
			if (ModelState.IsValid) {
				db.Entry(patron).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(patron);
		}

		// GET: Patrons/Delete/5
		public ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Patron patron = db.Patrons.Find(id);
			if (patron == null) {
				return HttpNotFound();
			}
			return View(patron);
		}

		// POST: Patrons/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id) {
			Patron patron = db.Patrons.Find(id);
			db.Patrons.Remove(patron);
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
