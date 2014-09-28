using AdminPortal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Tags {

	/**
	 * Retrieves the positions for each beacon assigned to a user at their
	 * current restaurant.
	 */
	public class TrackController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();
		private double MaxX = -1.0, MaxY = -1.0;

		private Transmitter[] LoadTransmitters(int restaurantId) {
			Transmitter[] t = new Transmitter[3];

			for (int i = 0; i < 3; i++) {
				t[i] = (from item in Db.Transmitters
						where item.Restaurant == restaurantId && item.Type == (i + 1)
						select item).FirstOrDefault();

				if (t[i] == null)
					return null;

				if (t[i].PosX > MaxX)
					MaxX = (double)t[i].PosX;

				if (t[i].PosY > MaxY) {
					MaxY = (double)t[i].PosY;
				}
			}

			return t;
		}

		[HttpPost]
		public IHttpActionResult GetLocations(RequestDetails details) {
			// Determine user's current restaurant:
			var user = (from item in Db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress)
						select item).FirstOrDefault();
			
			var restaurant = Db.Restaurants.Find(user.CurrentRestaurant);
			Transmitter[] t = LoadTransmitters(restaurant.ID);

			if (t == null)
				return BadRequest();

			Locator locator = new Locator();

			// Initialise transmitter locations.
			for (int i = 0; i < t.Length; i++)
				locator.MoveTransmitter(i + 1, (double)t[i].PosX, (double)t[i].PosY);

			// Load tags belonging to the user.
			var tags = from item in Db.Tags
					   where item.Restaurant == restaurant.ID && item.CurrentUser == user.ID
					   select item;

			var result = new List<TagData>();

			// For each of the user's tags"
			foreach (var tag in tags) {
				double[] strengths = new double[3];

				// Load all three of its strengths:
				for (int i = 0; i < 3; i++)
					strengths[i] = StrengthManager.GetStrength(tag.BeaconID, t[i].ID, (int)t[i].Type);

				// Triangulate its position:
				var pos = locator.Locate("", strengths[0], strengths[1], strengths[2]);

				TagData td = new TagData();
				td.Name = tag.Label;
				td.PosX = pos.X / MaxX;
				td.PosY = pos.Y / MaxY;
				result.Add(td);
			}

			return Ok(result);
		}

		public struct TagData {
			public string Name;
			public double PosX;
			public double PosY;
		}

		static TrackController() {
			StrengthManager.Update("1-177", 25, 1, -0.3);
			StrengthManager.Update("1-177", 65, 2, -0.3705523704);
			StrengthManager.Update("1-177", 66, 3, -0.65273907552);
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}
