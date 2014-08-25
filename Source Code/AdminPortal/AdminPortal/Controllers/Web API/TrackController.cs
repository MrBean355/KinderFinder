using System;
using System.Collections.Generic;
using System.Web.Http;

namespace AdminPortal.Controllers.Web_API {

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

	public class TrackController : ApiController {
		private static List<ChildSimulator> Children;

		static TrackController() {
			Children = new List<ChildSimulator>();

			for (int i = 0; i < 5; i++)
				Children.Add(new ChildSimulator());
		}

		[HttpPost]
		public IHttpActionResult GetLocations(RequestDetails details) {
			// TODO: Update this when locations are available.
			var result = new List<string>();

			foreach (var item in Children) {
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
