using AdminPortal.Models;

using System;
using System.Linq;

namespace AdminPortal.Code.Triangulation {

	public class Locator {
		private const int X = 0;
		private const int Y = 1;

		private KinderFinderEntities Db = new KinderFinderEntities();
		private double[][] Transmitters;

		public Locator() {
			Transmitters = new double[3][];

			for (int i = 0; i < Transmitters.Length; i++) {
				Transmitters[i] =  new double[2];
				Transmitters[i][X] = 0.0;
				Transmitters[i][Y] = 0.0;
			}
		}

		/// <summary>
		/// Sets the position of a transmitter.
		/// </summary>
		/// <param name="type">Type number of the transmitter.</param>
		/// <param name="x">Transmitter's new X co-ord.</param>
		/// <param name="y">Transmitter's new Y co-ord.</param>
		public void MoveTransmitter(int type, double x, double y) {
			if (type < 1 || type > 3) {
				System.Diagnostics.Debug.WriteLine("Locator: invalid type given (" + type + ").");
				return;
			}

			Transmitters[type - 1][X] = x;
			Transmitters[type - 1][Y] = y;
		}

		/// <summary>
		/// Attempts to trilaterate a position for a tag.
		/// </summary>
		/// <param name="beaconId">Tag's beacon ID.</param>
		/// <param name="str1">Transmitter 1's strength.</param>
		/// <param name="str2">Transmitter 2's strength.</param>
		/// <param name="str3">Transmitter 3's strength.</param>
		/// <returns>Tag's position if found; (-99, -99) otherwise.</returns>
		public Location Locate(string beaconId, double str1, double str2, double str3) {
			// Locate the tag:
			var transmitter = (from item in Db.Tags
							   where item.BeaconID.Equals(beaconId)
							   select item).FirstOrDefault();

			// Transmitter not found:
			if (transmitter == null) {
				System.Diagnostics.Debug.WriteLine("Locator: transmitter with UUID '" + beaconId + "' not found.");
				return new Location(-99.0, -99.0);
			}
			
			//var px = ((str1 * str1) - (str2 * str2) + (Nodes[1].X * Nodes[1].X)) / (2.0 * Nodes[1].X);
			//var py = ((str1 * str1) - (str3 * str3) + (Nodes[2].X * Nodes[2].X) + (Nodes[2].Y * Nodes[2].Y)) / (2.0 * Nodes[2].Y) - (Nodes[2].X / Nodes[2].X) * px;

			// Do some trilateration magic:
			var px = (Math.Pow(str1, 2.0) - Math.Pow(str2, 2.0) + (Math.Pow(Transmitters[1][X], 2.0))) / (2.0 * Transmitters[1][X]);
			var py = ((str1 * str1) - (str3 * str3) + (Transmitters[2][X] * Transmitters[2][X])
				+ (Transmitters[2][Y] * Transmitters[2][Y])) / (2.0 * Transmitters[2][Y]) - px;

			System.Diagnostics.Debug.WriteLine("Triangulating for " + beaconId + " with strengths:");
			System.Diagnostics.Debug.WriteLine("\t" + str1);
			System.Diagnostics.Debug.WriteLine("\t" + str2);
			System.Diagnostics.Debug.WriteLine("\t" + str3);
			System.Diagnostics.Debug.WriteLine("Result: (" + px + ", " + py + ")\n");

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