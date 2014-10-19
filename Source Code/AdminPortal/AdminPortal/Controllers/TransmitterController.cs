using AdminPortal.Code;
using AdminPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdminPortal.Controllers {

	[Authorize]
	public class TransmitterController : Controller {
		private KinderFinderEntities Db = new KinderFinderEntities();

		public ActionResult Index() {
			var restaurants = from item in Db.Restaurants
							  orderby item.Name
							  select item;

			var data = new Dictionary<string, Dictionary<int, long>>();

			foreach (var restaurant in restaurants) {
				var transmitters = from item in Db.Transmitters
								   where item.Restaurant == restaurant.ID
								   select item;

				var list = new Dictionary<int, long>();
				long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

				foreach (var transmitter in transmitters) {
					long last = StrengthManager.GetLastUpdated(transmitter.ID);
					list.Add(transmitter.ID, (now - last) / 1000);
				}

				data.Add(restaurant.Name, list);
			}

			ViewBag.TagInfo = data;

			return View(Db.Transmitters);
		}
	}
}