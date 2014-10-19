//#define MOCK_DATA

using AdminPortal.Code;
using AdminPortal.Code.Triangulation;
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
		/// <summary>
		/// Special value which indicates that the tag is out of range. This
		/// value is looked for by the Android app, and should NOT be changed
		/// without changing the corresponding value in the app's code.
		/// </summary>
		private const double OUT_OF_RANGE_VALUE = -100.0;
		/// <summary>
		/// Special value which indicates that a strength couldn't be loaded,
		/// meaning we couldn't locate the tag. This value is looked for by
		/// the Android app, and should NOT be changed without changing the
		/// corresponding value in the app's code.
		/// </summary>
		private const double MISSING_STRENGTH_VALUE = -50.0;

		private KinderFinderEntities Db = new KinderFinderEntities();
		private double MaxX = -1.0, MaxY = -1.0;

		//private static int debug_updates = 0;

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
			// Beacon IDs to test with:
			string[] beaconIds = { "1-177" , "1-209"};
			// Strengths to test with:
			double[] strengths = { 0.3, 0.3, 0.3, 0.1, 0.7, 0.7};

			// Generate temporary Transmitters (not saved in db):
			for (int i = 0; i < 3; i++) {
                t[i] = new Transmitter();
				t[i].ID = i + 1;
                t[i].Type = i + 1;
			}

			// Position Transmitters:
			t[0].PosX = 0.0; t[0].PosY = 0.0;
			t[1].PosX = 1.0; t[1].PosY = 0.0;
			t[2].PosX = 1.0; t[2].PosY = 1.0;

			// For each tag:
			for (int i = 0; i < beaconIds.Length; i++) {
				// Update tag strength to each transmitter:
				for (int j = 0; j < 3; j++)
					StrengthManager.Update(beaconIds[i], j + 1, j + 1, strengths[3*i+j]);
			}

            debug_updates++;
            System.Diagnostics.Debug.WriteLine("[Debug] Updates = " + debug_updates);

			if (debug_updates == 5)
				StrengthManager.FlagTag(beaconIds[0], true);
			else if (debug_updates == 10)
				StrengthManager.FlagTag(beaconIds[1], true);
			else if (debug_updates == 15)
				StrengthManager.FlagTag(beaconIds[1], false);
			else if (debug_updates == 20)
				StrengthManager.FlagTag(beaconIds[0], false);
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

		//[HttpPost] TODO: Uncomment.
		public IHttpActionResult GetLocations(RequestDetails details) {
			details = new RequestDetails();
			details.EmailAddress = "mrbean@gmail.com";
			// TODO: Remove above lines.

			// Determine user's current restaurant:
			var user = (from item in Db.AppUsers
						where item.EmailAddress.Equals(details.EmailAddress)
						select item).FirstOrDefault();
			
			var restaurant = Db.Restaurants.Find(user.CurrentRestaurant);

			if (restaurant == null)
				return BadRequest();

			// Load the restaurant's transmitters:
			Transmitter[] transmitters = LoadTransmitters(restaurant.ID);

			if (transmitters == null)
				return BadRequest();

			var locator = new Locator();

			// Tell the locator the transmitter positions:
			foreach (var transmitter in transmitters)
				locator.MoveTransmitter((int)transmitter.Type, (double)transmitter.PosX, (double)transmitter.PosY);

			// Load tags belonging to the user:
			var tags = from item in Db.Tags
					   where item.Restaurant == restaurant.ID && item.CurrentUser == user.ID
					   select new { item.Label, item.BeaconID };

			// List of tag positions to be returned:
			var result = new List<TagData>();

			// For each of the user's tags:
			foreach (var tag in tags) {
				// Tag has not been given a major-minor ID which corresponds to
				// the Bluetooth beacon's ID; we cannot locate it. This ID must
				// be set in the AdminPortal.
				if (tag.BeaconID == null || tag.BeaconID.Equals("")) {
					System.Diagnostics.Debug.WriteLine("[Warning] No beacon ID set for tag '" + tag.Label + "'.");
					continue;
				}

				var tagData = new TagData();
				tagData.Name = tag.Label;

                // Tag is not out of range; locate as normal:
                if (!StrengthManager.IsTagFlagged(tag.BeaconID)) {
                    double[] strengths = new double[3];
					bool safe = true; // set to false if a strength couldn't be determined.

                    // Load all three of its strengths:
					for (int i = 0; safe && i < 3; i++) {
						try {
							strengths[i] = StrengthManager.GetStrength(tag.BeaconID, transmitters[i].ID, (int)transmitters[i].Type);
						}
						catch (Exception ex) {
							System.Diagnostics.Debug.WriteLine("[Error] " + ex);
							safe = false;
						}
					}

					// ALl strengths loaded; calculate position:
					if (safe) {
						try {
							// Triangulate its position:
							var pos = locator.Locate(tag.BeaconID, strengths[0], strengths[1], strengths[2]);

							// Scale point to be between 0 and 1:
							tagData.PosX = pos.X / MaxX;
							tagData.PosY = pos.Y / MaxY;

							//var pos = Triangulate(strengths[0], strengths[1], strengths[2]);
							//td.PosX = pos.getXCoord() / MaxX;
							//td.PosY = pos.getYCoord() / MaxY;
						}
						catch (Exception ex) { // thrown if transmitter isn't in db.
							System.Diagnostics.Debug.WriteLine("[Error] " + ex);
							tagData.PosX = MISSING_STRENGTH_VALUE;
							tagData.PosY = MISSING_STRENGTH_VALUE;
						}
					}
					// A strength couldn't be loaded; can't locate:
					else {
						tagData.PosX = MISSING_STRENGTH_VALUE;
						tagData.PosY = MISSING_STRENGTH_VALUE;
					}
                }
				// Tag is out of range:
				else {
					tagData.PosX = OUT_OF_RANGE_VALUE;
					tagData.PosY = OUT_OF_RANGE_VALUE;
				}

				result.Add(tagData);
			}

			return Ok(result);
		}

        /*private Coordinates Triangulate(double str1, double str2, double str3) {
            //creating beacons
            Reciever r1 = new Reciever();
            Reciever r2 = new Reciever();
            Reciever r3 = new Reciever();

            //creating adapters
            AdapterToReciever adapter1 = new AdapterToReciever();
            AdapterToReciever adapter2 = new AdapterToReciever();
            AdapterToReciever adapter3 = new AdapterToReciever();

            //assiging reciever numbers
            adapter1.addRecieverNumber(1);
            adapter2.addRecieverNumber(2);
            adapter3.addRecieverNumber(3);

            //assigning signal strengths
            adapter1.addSignalStrength(str1);
            adapter2.addSignalStrength(str2);
            adapter3.addSignalStrength(str3);

            //assigning adapters
            r1 = adapter1.addReciever();
            r2 = adapter2.addReciever();
            r3 = adapter3.addReciever();

            //creating triagulation object
            Triangulate triangulate = new Triangulate(10, 11);

            //adding beacons...
            triangulate.add3Recievers(r1, r2, r3);

            //creating matrix
            triangulate.createMatrix();

            //showing empty matrix            
            //triangulate.printArray();

            //triagulating
            triangulate.triangulateCoord();
            
            //creating coordinates point for the rest of the program
            //Coordinates coordinates = new Coordinates();
            return triangulate.getCoordinatesForAdapter();
        }*/

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
