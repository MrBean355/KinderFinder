#define MOCK_DATA

using AdminPortal.Code;
using AdminPortal.Code.Triangulation;
using AdminPortal.Models;

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

#if !MOCK_DATA
			for (int i = 0; i < 3; i++) {
				t[i] = (from item in Db.Transmitters
						where item.Restaurant == restaurantId && item.Type == (i + 1)
						select item).FirstOrDefault();

				if (t[i] == null)
					return null;
			}
#else
			var restaurant = (from item in Db.Restaurants
							  where item.Name.Equals("Demo Room")
							  select item.ID).FirstOrDefault();

			for (int i = 0; i < 3; i++) {
                t[i] = new Transmitter();
				t[i].ID = i + 1;
                t[i].Type = i + 1;
				t[i].Restaurant = restaurant;
			}

			t[0].PosX =  0.0; t[0].PosY =  0.0;
			t[1].PosX = 10.0; t[1].PosY =  0.0;
			t[2].PosX = 10.0; t[2].PosY = 10.0;

			StrengthManager.Update("1-177", 1, 1, -0.3);
			StrengthManager.Update("1-177", 2, 2, -0.0);
			StrengthManager.Update("1-177", 3, 3, -0.3);
            updates++;
            System.Diagnostics.Debug.WriteLine("Updates " + updates);
            if (updates == 10)
                StrengthManager.FlagTag("1-177", true);
#endif
			// Find the max and min co-ords, so we can scale the points:
			foreach (var item in t) {
				if (item.PosX > MaxX)
					MaxX = (double)item.PosX;

				if (item.PosY > MaxY)
					MaxY = (double)item.PosY;
			}

			return t;
		}
        static int updates = 0;
		[HttpPost]
		public IHttpActionResult GetLocations(RequestDetails details) {
			// Determine user's current restaurant:
			var user = (from item in Db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress)
						select item).FirstOrDefault();
			
			var restaurant = Db.Restaurants.Find(user.CurrentRestaurant);

			if (restaurant == null)
				return BadRequest();

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

			// List of tag positions to be returned:
			var result = new List<TagData>();

			// For each of the user's tags:
			foreach (var tag in tags) {
                TagData td = new TagData();
                td.Name = tag.Label;
                var pos = new Locator.Location(-100.0, -100.0);

                // Tag is not flagged; locate as normal:
                if (!StrengthManager.IsTagFlagged(tag.BeaconID)) {
                    double[] strengths = new double[3];

                    // Load all three of its strengths:
                    for (int i = 0; i < 3; i++) {
                        //System.Diagnostics.Debug.WriteLine("Here: " + t[i].ID);
                        //System.Diagnostics.Debug.WriteLine("Here: " + (int)t[i].Type);
                        strengths[i] = StrengthManager.GetStrength(tag.BeaconID, t[i].ID, (int)t[i].Type);

                        if (strengths[i] == StrengthManager.NOT_ENOUGH_AVERAGES) {
                            // TODO: Do something if there aren't enough averages?
                        }
                    }

                    // Triangulate its position:
                    pos = locator.Locate(tag.BeaconID, strengths[0], strengths[1], strengths[2]);
                    td.PosX = pos.X / MaxX;
                    td.PosY = pos.Y / MaxY;
                }
                else {
                    td.PosX = pos.X;
                    td.PosY = pos.Y;
                }
				result.Add(td);
			}

			return Ok(result);
		}

		public struct TagData {
			public string Name;
			public double PosX;
			public double PosY;
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}
