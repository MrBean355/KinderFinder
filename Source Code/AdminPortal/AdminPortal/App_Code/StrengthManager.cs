using System.Collections.Generic;

namespace AdminPortal.Code {

	public static class StrengthManager {
		private static Dictionary<string, Dictionary<int, List<double>>> Awe = new Dictionary<string, Dictionary<int, List<double>>>();

		public static void Update(string uuid, int tId, int tType, double strength) {
			if (!Awe.ContainsKey(uuid)) {
				var item = new Dictionary<int, List<double>>();
				Awe.Add(uuid, item);
			}

			if (!Awe[uuid].ContainsKey(tId)) {
				var item = new List<double>();
				item.Add(-0.99);
				item.Add(-0.99);
				item.Add(-0.99);
				Awe[uuid].Add(tId, item);
			}

			System.Diagnostics.Debug.WriteLine("Updated: Tag: " + uuid + ", Trans: " + tId + ", Type: " + tType);
			Awe[uuid][tId][tType - 1] = strength;
		}

		public static double GetStrength(string uuid, int tId, int tType) {
			if (!Awe.ContainsKey(uuid)) {
				System.Diagnostics.Debug.WriteLine("Unable to locate strength for tag with UUID '" + uuid + "'.");
				return -98.0;
			}

			if (!Awe[uuid].ContainsKey(tId)) {
				System.Diagnostics.Debug.WriteLine("Unable to locate strength for tag with UUID '" + uuid + "' and transmitter with ID '" + tId + "'.");
				return -97.0;
			}

			return Awe[uuid][tId][tType - 1];
		}
	}

	public struct Strength {
		public string TagUuid;
		public double Distance;
	}
}