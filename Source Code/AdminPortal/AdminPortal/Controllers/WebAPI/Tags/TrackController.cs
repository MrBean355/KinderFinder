using AdminPortal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Tags {

	class ChildSimulator {
		private static Random Generator = new Random(DateTime.Now.Millisecond);
		private static double TheIncrement = 0.01;

		public double X, Y;
		private bool PosX, PosY;

		public ChildSimulator() {
			X = Generator.NextDouble();
			Y = Generator.NextDouble();

			PosX = Generator.NextDouble() >= 0.5;
			PosY = Generator.NextDouble() >= 0.5;
		}

		public void Increment() {
			if (PosX)
				X += TheIncrement;
			else
				X -= TheIncrement;

			if (PosY)
				Y += TheIncrement;
			else
				Y -= TheIncrement;

			if (X > 1.0) {
				X = 1.0;
				PosX = !PosX;
			}
			else if (X < 0.0) {
				X = 0.0;
				PosX = !PosX;
			}

			if (Y > 1.0) {
				Y = 1.0;
				PosY = !PosY;
			}
			else if (Y < 0.0) {
				Y = 0.0;
				PosY = !PosY;
			}
		}
	}

	/**
	 * Retrieves the positions for each beacon assigned to a user at their
	 * current restaurant.
	 */
	public class TrackController : ApiController {
		private const int MAX_TAGS = 50;
		private static List<ChildSimulator> Children;

		private KinderFinderEntities Db = new KinderFinderEntities();

		static TrackController() {
			Children = new List<ChildSimulator>();

			for (int i = 0; i < MAX_TAGS; i++)
				Children.Add(new ChildSimulator());
		}

		/**
		 * TODO: Update this function to return actual locations.
		 */
		[HttpPost]
		public IHttpActionResult GetLocations(RequestDetails details) {
			// Determine user's current restaurant:
			var restaurant = (from item in Db.AppUsers
							  where item.EmailAddress.Equals(details.EmailAddress)
							  select item.Restaurant).FirstOrDefault();

			// Count how many tags they have at the restaurant:
			var tags = (from item in Db.Tags
						where item.AppUser.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
							&& item.Restaurant == restaurant.ID
						select item).Count();

			var result = new List<string>();

			for (int i = 0; i < Children.Count; i++) {
				if (i >= tags)
					break;

				ChildSimulator item = Children[i];
				result.Add(item.X.ToString());
				result.Add(item.Y.ToString());
				item.Increment();
			}

			return Ok(result);
		}

		public struct RequestDetails {
			public string EmailAddress;
		}
	}
}
