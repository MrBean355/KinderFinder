using System;
using System.Collections.Generic;

namespace AdminPortal.Code {

	public static class StrengthManager {
		public const int AVERAGE_VALUES = 5;
		public const double NOT_ENOUGH_AVERAGES = -95.0;

		private static Dictionary<string, Dictionary<int, List<AverageManager>>> Strengths = new Dictionary<string, Dictionary<int, List<AverageManager>>>();

		public static void Update(string uuid, int tId, int tType, double strength) {
			// This tag has not been added yet; insert it:
			if (!Strengths.ContainsKey(uuid)) {
				var item = new Dictionary<int, List<AverageManager>>();
				Strengths.Add(uuid, item);
			}

			// This transmitter has not been added for this tag; insert it:
			if (!Strengths[uuid].ContainsKey(tId)) {
				var item = new List<AverageManager>();
				// Insert an AverageManager for each transmitter type.
				item.Add(new AverageManager());
				item.Add(new AverageManager());
				item.Add(new AverageManager());
				Strengths[uuid].Add(tId, item);
			}

			System.Diagnostics.Debug.WriteLine("Strength updated from type " + tType);
			Strengths[uuid][tId][tType - 1].AddStrength(strength);
		}

		public static double GetStrength(string uuid, int tId, int tType) {
			// This tag has not been added; error:
			if (!Strengths.ContainsKey(uuid)) {
				System.Diagnostics.Debug.WriteLine("Unable to locate strength for tag with UUID '" + uuid + "'.");
				return -98.0;
			}

			// This transmitter has not been added for this tag; error:
			if (!Strengths[uuid].ContainsKey(tId)) {
				System.Diagnostics.Debug.WriteLine("Unable to locate strength for tag with UUID '" + uuid + "' and transmitter with ID '" + tId + "'.");
				return -97.0;
			}

			// Find the average manager:
			var manager = Strengths[uuid][tId][tType - 1];

			// Enough values have been read for an average:
			if (manager.CountAverages() >= AVERAGE_VALUES) {
				// Get the average and reset the averages:
				double result = manager.GetAverage();
				manager.ResetAverages();

				return result;
			}

			// Not enough values have been read for an average:
			return NOT_ENOUGH_AVERAGES;
		}
	}
}