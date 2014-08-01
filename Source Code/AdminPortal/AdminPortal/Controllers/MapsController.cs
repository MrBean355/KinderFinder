using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdminPortal.Models;
using System.IO;
using System.Drawing;

namespace AdminPortal.Controllers {
	public class MapsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		// GET: Maps
		public ActionResult Index() {
			return View(db.Maps.ToList());
		}

		public FileContentResult GetImage(int? id) {
			if (id == null)
				return null;

			Map map = db.Maps.Find(id);

			if (map == null || map.Data == null)
				return null;

			return new FileContentResult(map.Data, "image/jpeg");
		}

		public ActionResult DeleteImage(int? id) {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			Map map = db.Maps.Find(id);

			if (map == null)
				return HttpNotFound();

			map.Data = null;
			db.SaveChanges();

			return RedirectToAction("Edit", new { id = id });
		}

		// GET: Maps/Details/5
		public ActionResult Details(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Map map = db.Maps.Find(id);
			if (map == null) {
				return HttpNotFound();
			}
			return View(map);
		}

		// GET: Maps/Create
		public ActionResult Create() {
			return View();
		}

		// POST: Maps/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Name,Data,Active")] Map map, HttpPostedFileBase file) {
			if (ModelState.IsValid) {
				if (file != null) {
					MemoryStream target = new MemoryStream();
					file.InputStream.CopyTo(target);
					map.Data = target.ToArray();
				}

				/* Make any active map not active. */
				if (map.Active) {
					var maps = from item in db.Maps
							   where item.Active
							   select item;

					foreach (var m in maps)
						m.Active = false;
				}

				db.Maps.Add(map);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(map);
		}

		// GET: Maps/Edit/5
		public ActionResult Edit(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Map map = db.Maps.Find(id);
			if (map == null) {
				return HttpNotFound();
			}
			return View(map);
		}

		// POST: Maps/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Name,Data,Active")] Map map, HttpPostedFileBase file) {
			if (ModelState.IsValid) {
				if (file != null) {
					MemoryStream target = new MemoryStream();
					file.InputStream.CopyTo(target);
					map.Data = target.ToArray();
				}

				/* Make any active map not active. */
				if (map.Active) {
					var maps = from item in db.Maps
							   where item.Active
							   select item;

					foreach (var m in maps)
						m.Active = false;
				}

				db.Entry(map).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(map);
		}

		// GET: Maps/Delete/5
		public ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Map map = db.Maps.Find(id);
			if (map == null) {
				return HttpNotFound();
			}
			return View(map);
		}

		// POST: Maps/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id) {
			Map map = db.Maps.Find(id);
			db.Maps.Remove(map);
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
