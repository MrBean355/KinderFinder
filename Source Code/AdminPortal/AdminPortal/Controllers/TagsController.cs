﻿using AdminPortal.Models;

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdminPortal.Controllers {

	[Authorize]
	public class TagsController : Controller {
		private KinderFinderEntities db = new KinderFinderEntities();

		/// <summary>
		/// Determines whether the tag with specified ID is accessible by the
		/// currently logged in user.
		/// </summary>
		/// <param name="id">ID of the tag.</param>
		/// <returns>True if accessible; false otherwise.</returns>
		private bool IsTagAccessible(int? id) {
			if (id != null && !User.IsInRole("GlobalAdmins")) {
				var query = (from user in db.AspNetUsers
							join rest in db.Restaurants on user.Id equals rest.Admin
							join tag in db.Tags on rest.ID equals tag.Restaurant
							where user.UserName.Equals(User.Identity.Name) && tag.ID == id
							select tag).FirstOrDefault();

				return query != null;
			}

			return true;
		}

		// GET: Tags
		public ActionResult Index() {
			if (User.IsInRole("GlobalAdmins"))
				return View(db.Tags.ToList());

			var result = new List<Tag>();

			// Fetches all tags that belong to the current admin.
			// Basically gets all tags owned by restaurants that the user is an admin of.
			var query = from user in db.AspNetUsers
						join rest in db.Restaurants on user.Id equals rest.Admin
						join tag in db.Tags on rest.ID equals tag.Restaurant
						where user.UserName.Equals(User.Identity.Name)
						select tag;

			if (query != null)
				result.AddRange(query);

			return View(result);
		}

		// GET: Tags/Details/5
		public ActionResult Details(int? id) {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			if (!IsTagAccessible(id))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			Tag tag = db.Tags.Find(id);

			if (tag == null)
				return HttpNotFound();

			return View(tag);
		}

		// GET: Tags/Create
		public ActionResult Create() {
			ViewBag.CurrentUser = new SelectList(db.AppUsers, "ID", "FirstName");

			if (User.IsInRole("GlobalAdmins"))
				ViewBag.Restaurant = new SelectList(db.Restaurants, "ID", "Name");
			else {
				// If the user is not a global admin, they can only assign tags
				// to restaurants that they are "admin-ing".
				var list = new List<Restaurant>();

				var query = from user in db.AspNetUsers
							join rest in db.Restaurants on user.Id equals rest.Admin
							where user.UserName.Equals(User.Identity.Name)
							select rest;

				if (query != null)
					list.AddRange(query);

				ViewBag.Restaurant = new SelectList(list, "ID", "Name");
			}

			return View();
		}

		// POST: Tags/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Label,Restaurant,CurrentUser")] Tag tag) {
			if (ModelState.IsValid) {
				db.Tags.Add(tag);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(tag);
		}

		// GET: Tags/Edit/5
		public ActionResult Edit(int? id) {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			if (!IsTagAccessible(id))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			Tag tag = db.Tags.Find(id);

			if (tag == null)
				return HttpNotFound();

			ViewBag.CurrentUser = new SelectList(db.AppUsers, "ID", "FirstName", tag.CurrentUser);

			if (User.IsInRole("GlobalAdmins"))
				ViewBag.Restaurant = new SelectList(db.Restaurants, "ID", "Name");
			else {
				// If the user is not a global admin, they can only assign tags
				// to restaurants that they are "admin-ing".
				var list = new List<Restaurant>();

				var query = from user in db.AspNetUsers
							join rest in db.Restaurants on user.Id equals rest.Admin
							where user.UserName.Equals(User.Identity.Name)
							select rest;

				if (query != null)
					list.AddRange(query);

				ViewBag.Restaurant = new SelectList(list, "ID", "Name");
			}

			return View(tag);
		}

		// POST: Tags/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Label,Restaurant,CurrentUser")] Tag tag) {
			if (ModelState.IsValid) {
				db.Entry(tag).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(tag);
		}

		// GET: Tags/Delete/5
		public ActionResult Delete(int? id) {
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			if (!IsTagAccessible(id))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

			Tag tag = db.Tags.Find(id);

			if (tag == null)
				return HttpNotFound();

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
