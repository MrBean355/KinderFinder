using AdminPortal.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI {

	public class TestController : ApiController {

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

				if (t[i].PosY > MaxY)
					MaxY = (double)t[i].PosY;
			}

			return t;
		}

		//[HttpPost]
		public IEnumerable GetLocations() {
			// TODO: Remove.
			StrengthManager.Update("1-777", 73, 1, -0.6);
			System.Diagnostics.Debug.WriteLine("Test strength: " + StrengthManager.GetStrength("1-777", 73, 1));

			// Determine user's current restaurant:
			var user = (from item in Db.AppUsers
						where item.EmailAddress.Equals("mrbean@gmail.com")
						select item).FirstOrDefault();

			var restaurant = Db.Restaurants.Find(user.CurrentRestaurant);

			if (restaurant == null) {
				System.Diagnostics.Debug.WriteLine("Restaurant not found");
				return null;// BadRequest();
			}

			Transmitter[] t = LoadTransmitters(restaurant.ID);

			if (t == null) {
				System.Diagnostics.Debug.WriteLine("Transmitters not found");
				return null;// BadRequest();
			}

			Locator locator = new Locator();

			// Initialise transmitter locations.
			for (int i = 0; i < t.Length; i++)
				locator.MoveTransmitter(i + 1, (double)t[i].PosX, (double)t[i].PosY);

			// Load tags belonging to the user.
			var tags = from item in Db.Tags
					   where item.Restaurant == restaurant.ID && item.CurrentUser == user.ID
					   select item;

			// List of tag positions to be returned:
			var result = new List<TagData>();

			// For each of the user's tags"
			foreach (var tag in tags) {
				double[] strengths = new double[3];

				// Load all three of its strengths:
				for (int i = 0; i < 3; i++) {
					System.Diagnostics.Debug.WriteLine("Searching for "+ tag.BeaconID + ", " + t[i].ID + ", " + t[i].Type);
					strengths[i] = StrengthManager.GetStrength(tag.BeaconID, t[i].ID, (int)t[i].Type);
				}

				// Triangulate its position:
				var pos = locator.Locate(tag.BeaconID, strengths[0], strengths[1], strengths[2]);

				TagData td = new TagData();
				td.Name = tag.Label;
				td.PosX = pos.X / MaxX;
				td.PosY = pos.Y / MaxY;
				result.Add(td);
			}

			//return Ok(result);

			System.Diagnostics.Debug.WriteLine("Returning " + result.ToArray().ToString());
			return result;
		}

		public struct TagData {
			public string Name;
			public double PosX;
			public double PosY;
		}
	}
}