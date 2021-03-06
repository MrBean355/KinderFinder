﻿using AdminPortal.Models;

using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Transmitters {
	
	/**
	 * Assigns a transmitter with a type to a restaurant.
	 */
	public class AssignRestaurantController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult AssignRestaurant(RequestDetails details) {
			var restaurant = (from item in Db.Restaurants
							  where item.Name.Equals(details.RestaurantName)
							  select item).FirstOrDefault();

			if (restaurant == null)
				return BadRequest();
			
			int type = int.Parse(details.TransmitterType);

			var count = (from item in Db.Transmitters
						 where item.Restaurant == restaurant.ID && item.Type == type
						 select item).Count();

			if (count > 0)
				return Conflict();

			Transmitter t = new Transmitter();

			//details.X = details.X.Replace('.', ',');
			//details.Y = details.Y.Replace('.', ',');

			t.Type = int.Parse(details.TransmitterType);
			t.PosX = float.Parse(details.X);
			t.PosY = float.Parse(details.Y);
			t.Restaurant = restaurant.ID;

			Db.Transmitters.Add(t);
			Db.SaveChanges();

			System.Diagnostics.Debug.WriteLine("[Info] Transmitter " + t.ID + " added to system. Restaurant: " + restaurant.Name + "; type: " + t.Type + ".");

			return Ok(t.ID);
		}

		public struct RequestDetails {
			public string RestaurantName;
			public string TransmitterType;
			public string X, Y;
		}
	}
}