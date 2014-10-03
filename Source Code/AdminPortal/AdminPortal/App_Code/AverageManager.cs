using System;
using System.Collections.Generic;

namespace AdminPortal.Code {
	
	public class AverageManager {
		private List<double> Data = new List<double>();

		public void AddStrength(double strength) {
			Data.Add(strength);
		}

		public double GetAverage() {
			if (Data.Count == 0) {
				System.Diagnostics.Debug.WriteLine("Error getting average strength: no strengths to average.");
				return -1.0;
			}

			double sum = 0.0;

			foreach (var item in Data) {
				sum += item;
			}

			double average = sum / Data.Count;
			double dist = double.MaxValue;
			double best = -1.0;

			foreach (var item in Data) {
				double diff = Math.Abs(item - average);

				if (diff < dist) {
					dist = diff;
					best = item;
				}
			}

			return best;
		}

		public int CountAverages() {
			return Data.Count;
		}

		public void ResetAverages() {
			Data.Clear();
		}
	}
}