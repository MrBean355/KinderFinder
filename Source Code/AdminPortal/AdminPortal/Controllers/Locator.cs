using AdminPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminPortal.Controllers {

	public class Locator {
		private const int X = 0;
		private const int Y = 1;

		private KinderFinderEntities Db = new KinderFinderEntities();
		private double[][] Transmitters;

		public Locator() {
			Transmitters = new double[3][];

			for (int i = 0; i < Transmitters.Length; i++) {
				Transmitters[i] = new double[2];
				Transmitters[i][X] = 0.0;
				Transmitters[i][Y] = 0.0;
			}
		}

		public void MoveTransmitter(int type, double x, double y) {
			if (type < 1 || type > 3) {
				System.Diagnostics.Debug.WriteLine("Locator: invalid type given (" + type + ").");
				return;
			}

			Transmitters[type - 1][X] = x;
			Transmitters[type - 1][Y] = y;
		}

		public Location Locate(string uuid, double str1, double str2, double str3) {
			var transmitter = (from item in Db.Tags
							   //where item.UUID.Equals(uuid)
							   select item).FirstOrDefault();

			if (transmitter == null) {
				System.Diagnostics.Debug.WriteLine("Locator: transmitter with UUID '" + uuid + "' not found.");
				return new Location(-99.0, -99.0);
			}
			
			//var px = ((str1 * str1) - (str2 * str2) + (Nodes[1].X * Nodes[1].X)) / (2.0 * Nodes[1].X);
			//var py = ((str1 * str1) - (str3 * str3) + (Nodes[2].X * Nodes[2].X) + (Nodes[2].Y * Nodes[2].Y)) / (2.0 * Nodes[2].Y) - (Nodes[2].X / Nodes[2].X) * px;

			var px = (Math.Pow(str1, 2.0) - Math.Pow(str2, 2.0) + (Math.Pow(Transmitters[1][X], 2.0))) / (2.0 * Transmitters[1][X]);
			var py = ((str1 * str1) - (str3 * str3) + (Transmitters[2][X] * Transmitters[2][X])
				+ (Transmitters[2][Y] * Transmitters[2][Y])) / (2.0 * Transmitters[2][Y]) - px;
			
			return new Location(px, py);
		}

		public struct Location {
			public double X;
			public double Y;

			public Location(double x, double y) {
				X = x;
				Y = y;
			}
		}
	}
}