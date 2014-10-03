using AdminPortal.Code;
using AdminPortal.Models;
using AdminPortal.Triangulation;
//using triangulation_alpha0._2;


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

				if (t[i].PosY > MaxY)
					MaxY = (double)t[i].PosY;
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

			// For each of the user's tags"
			foreach (var tag in tags) {
				double[] strengths = new double[3];

				// Load all three of its strengths:
				for (int i = 0; i < 3; i++)
					strengths[i] = StrengthManager.GetStrength(tag.BeaconID, t[i].ID, (int)t[i].Type);

				// Triangulate its position:
				var pos = locator.Locate(tag.BeaconID, strengths[0], strengths[1], strengths[2]);
				//var coord = Triangulate((float)strengths[0], (float)strengths[1], (float)strengths[2]);

				TagData td = new TagData();
				td.Name = tag.Label;
				td.PosX = pos.X / MaxX;
				td.PosY = pos.Y / MaxY;
				//td.PosX = coord.getXCoord() / MaxX;
				//td.PosY = coord.getYCoord() / MaxY;
				result.Add(td);
			}

			return Ok(result);
		}

		// TODO: Be able to set transmitter locations.
		// From [0, 1] to actual co-ords (meters).
		private Coordinates TriangulateStuff(float s1, float s2, float s3) {
			//creating beacons
			Reciever b1 = new Reciever();
			Reciever b2 = new Reciever();
			Reciever b3 = new Reciever();

			//creating adapters
			AdapterToReciever adapter1 = new AdapterToReciever();
			AdapterToReciever adapter2 = new AdapterToReciever();
			AdapterToReciever adapter3 = new AdapterToReciever();

			//assiging signal strengths
			adapter1.addBeaconNumber(1);
			adapter2.addBeaconNumber(2);
			adapter3.addBeaconNumber(3);

			//assigning signal strengths
			adapter1.addSignalStrength(s1);
			adapter2.addSignalStrength(s2);
			adapter3.addSignalStrength(s3);

			//assigning adapters
			b1 = adapter1.addBeacon();
			b2 = adapter2.addBeacon();
			b3 = adapter3.addBeacon();

			//creating triagulation object
			Triangulate triangulate = new Triangulate();

			//adding beacons...
			triangulate.add3Beacons(b1, b2, b3);

			//creating matrix
			triangulate.createMatrix();

			//showing empty matrix            
			triangulate.printArray();

			//triagulating
			triangulate.triangulateCoord();

			//creating coordinates point for the rest of the program
			Coordinates coordinates = new Coordinates();
			coordinates = triangulate.getCoordinatesForAdapter();
			return coordinates;
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
