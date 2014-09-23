using AdminPortal.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System;

namespace AdminPortal.Controllers.Web_API {

	public class TransmitController : ApiController {
		
		[HttpPost]
		public IHttpActionResult Transmit(List<RequestDetails> details) {
			// TODO: Do something with received strengths.
			foreach (var item in details) {
				System.Diagnostics.Debug.WriteLine(item.id + ": " + item.distance);
			}

			return Ok();
		}

		public struct RequestDetails {
			public string id;
			public float distance;
		}
	}

	public class FreeTransmitterController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult FreeTransmitter(RequestDetails details) {
			int id = int.Parse(details.ID);
			var transmitter = Db.Transmitters.Find(id);

			if (transmitter == null) {
				return BadRequest();
			}

			Db.Transmitters.Remove(transmitter);
			Db.SaveChanges();

			return Ok();
		}

		public struct RequestDetails {
			public string ID;
		}
	}

	public class AssignRestaurantController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult AssignRestaurant(RequestDetails details) {
			var restaurant = (from item in Db.Restaurants
							  where item.Name.Equals(details.RestaurantName)
							  select item).FirstOrDefault();

			if (restaurant == null) {
				return BadRequest();
			}
			
			int type = int.Parse(details.TransmitterType);

			var count = (from item in Db.Transmitters
						 where item.Restaurant == restaurant.ID && item.Type == type
						 select item).Count();

			if (count > 0) {
				return BadRequest();
			}

			Transmitter t = new Transmitter();
			
			t.Type = int.Parse(details.TransmitterType);
			t.PosX = float.Parse(details.X);
			t.PosY = float.Parse(details.Y);
			t.Restaurant = restaurant.ID;
			
			Db.Transmitters.Add(t);
			Db.SaveChanges();

			return Ok(t.ID);
		}

		public struct RequestDetails {
			public string RestaurantName;
			public string TransmitterType;
			public string X, Y;
		}
	}

	public class TransmitterTypeController : ApiController {
		private KinderFinderEntities Db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult GetAvailableTypes(RequestDetails details) {
			var restaurant = (from item in Db.Restaurants
							 where item.Name.Equals(details.RestaurantName)
							 select item).FirstOrDefault();

			if (restaurant == null) {
				return BadRequest();
			}

			var types = new List<string>();
			types.Add("1");
			types.Add("2");
			types.Add("3");

			var trans = from item in Db.Transmitters
						where item.Restaurant == restaurant.ID
						select item;

			foreach (var item in trans) {
				string type = (int)item.Type + "";

				if (types.Contains(type)) {
					types.Remove(type);
				}
			}

			if (types.Count == 0) {
				return BadRequest();
			}

			return Ok(types);
		}

		public struct RequestDetails {
			public string RestaurantName;
		}
	}
}