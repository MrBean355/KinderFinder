using System.Collections.Generic;

namespace AdminPortal.Code {

	public static class StrengthManager {
		public const int AVERAGE_VALUES = 1;

		//                       BeaconID          TransID   Avg for each trans type
		private static Dictionary<string, Dictionary<int, List<AverageManager>>> Strengths;
		private static List<string> FlaggedTags;

		static StrengthManager() {
			Strengths = new Dictionary<string, Dictionary<int, List<AverageManager>>>();
			FlaggedTags = new List<string>();
		}

        /// <summary>
        /// Flags a tag as being out of range of one of the transmitters.
        /// </summary>
        /// <param name="beaconId">Beacon ID of the tag (in major-minor format).</param>
        /// <param name="flag">Flag mode.</param>
        public static void FlagTag(string beaconId, bool flag) {
            if (flag) {
                if (!FlaggedTags.Contains(beaconId))
                    FlaggedTags.Add(beaconId);
            }
            else {
                if (FlaggedTags.Contains(beaconId))
                    FlaggedTags.Remove(beaconId);
            }
        }

        /// <summary>
        /// Checks whether a tag has been flagged as out of range of one of the transmitters.
        /// </summary>
        /// <param name="beaconId">Beacon ID of the tag (in major-minor format).</param>
        /// <returns>True if it's flagged; false otherwise.</returns>
        public static bool IsTagFlagged(string beaconId) {
            return FlaggedTags.Contains(beaconId);
        }

        /// <summary>
        /// Updates the strength value between a tag and a transmitter.
        /// </summary>
        /// <param name="tagMinorMajor">Minor-major value of the tag.</param>
        /// <param name="tId">Transmitter ID.</param>
        /// <param name="tType">Transmitter type.</param>
        /// <param name="strength">Strength value.</param>
		public static void Update(string tagMinorMajor, int tId, int tType, double strength) {
			// This tag has not been added yet; insert it:
			if (!Strengths.ContainsKey(tagMinorMajor)) {
				var item = new Dictionary<int, List<AverageManager>>();
				Strengths.Add(tagMinorMajor, item);
			}

			// This transmitter has not been added for this tag; insert it:
			if (!Strengths[tagMinorMajor].ContainsKey(tId)) {
				var item = new List<AverageManager>();
				// Insert an AverageManager for each transmitter type.
				item.Add(new AverageManager(AVERAGE_VALUES));
				item.Add(new AverageManager(AVERAGE_VALUES));
				item.Add(new AverageManager(AVERAGE_VALUES));
				Strengths[tagMinorMajor].Add(tId, item);
			}

			//System.Diagnostics.Debug.WriteLine("[Info] Strength updated for beacon " + tagMinorMajor + " from type " + tType);
			Strengths[tagMinorMajor][tId][tType - 1].AddStrength(strength);
		}

		public static double GetStrength(string tagMinorMajor, int tId, int tType) {
			// This tag has not been added; error:
			if (!Strengths.ContainsKey(tagMinorMajor)) {
				System.Diagnostics.Debug.WriteLine("[Warning] Unable to locate strength for tag with minor-major value '" + tagMinorMajor + "'.");
				return -98.0;
			}

			// This transmitter has not been added for this tag; error:
			if (!Strengths[tagMinorMajor].ContainsKey(tId)) {
                System.Diagnostics.Debug.WriteLine("[Warning] Unable to locate strength for tag with minor-major value '" + tagMinorMajor + "' and transmitter with ID '" + tId + "'.");
				return -97.0;
			}

			// Find the average from the average manager:
			var manager = Strengths[tagMinorMajor][tId][tType - 1];
			return manager.GetAverage();
		}
	}
}