using AdminPortal.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdminPortal.Controllers {
	[Authorize] // Must be logged in to access any of these pages.
	public class TagsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		// GET: Tags
		public ActionResult Index() {
			var tags = db.Tags.Include(t => t.Patron);
			return View(tags.ToList());
		}

		// GET: Tags/Create
		public ActionResult Create() {
			ViewBag.CurrentPatron = new SelectList(db.Patrons, "ID", "FirstName");
			return View();
		}

		// POST: Tags/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,CurrentPatron,Label")] Tag tag) {
			if (ModelState.IsValid) {
				db.Tags.Add(tag);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.CurrentPatron = new SelectList(db.Patrons, "ID", "FirstName", tag.CurrentPatron);
			return View(tag);
		}

		// GET: Tags/Edit/5
		public ActionResult Edit(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tag tag = db.Tags.Find(id);
			if (tag == null) {
				return HttpNotFound();
			}
			ViewBag.CurrentPatron = new SelectList(db.Patrons, "ID", "FirstName", tag.CurrentPatron);
			return View(tag);
		}

		// POST: Tags/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,CurrentPatron,Label")] Tag tag) {
			if (ModelState.IsValid) {
				db.Entry(tag).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.CurrentPatron = new SelectList(db.Patrons, "ID", "FirstName", tag.CurrentPatron);
			return View(tag);
		}

		// GET: Tags/Delete/5
		public ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tag tag = db.Tags.Find(id);
			if (tag == null) {
				return HttpNotFound();
			}
			return View(tag);
		}

		// POST: Tags/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id) {
			Tag tag = db.Tags.Find(id);
			db.Tags.Remove(tag);
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
