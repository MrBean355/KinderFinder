using System;
using System.Collections.Generic;

namespace AdminPortal.Code {
	
	public class AverageManager {
		private const double NOT_SET_VALUE = -10.0;
		private const double STARTING_BEST = -100.0;

		private List<double> Data;
		private int Size;
		private int Index;

		public AverageManager(int averages) {
			Data = new List<double>();
			Size = averages;
			Index = 0;

			for (int i = 0; i < Size; i++)
				Data.Add(NOT_SET_VALUE);
		}

		public void AddStrength(double strength) {
			if (strength < 0.0)
				return;

			Data[Index] = strength;
			Index = (Index + 1) % Size;
		}

		public double GetAverage() {
			// First, calculate the average of all the strengths:
			double average = 0.0;

			foreach (var strength in Data)
				average += strength;

			average /= Size;
			double bestDiff = 0.0;
			double bestStrength = STARTING_BEST;
			string output = "Choosing from [ ";

			// Next we need to find which strength is closest to the average:
			foreach (var strength in Data) {
				double diff = Math.Abs(average - strength);
				output += strength + " ";

				if (bestStrength == STARTING_BEST || diff < bestDiff) {
					if (strength == NOT_SET_VALUE)
						continue;

					bestStrength = strength;
					bestDiff = diff;
				}
			}

			output += "] = " + bestStrength;
			System.Diagnostics.Debug.WriteLine(output);
			return bestStrength;
		}
	}
}