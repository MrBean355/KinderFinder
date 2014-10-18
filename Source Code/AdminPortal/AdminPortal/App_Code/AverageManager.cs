using System;
using System.Collections.Generic;

namespace AdminPortal.Code {
	
	public class AverageManager {
		private List<double> Data;
		private int Size;
		private int Index;

		public AverageManager(int averages) {
			Data = new List<double>();
			Size = averages;
			Index = 0;

			for (int i = 0; i < Size; i++)
				Data.Add(0.0);
		}

		public void AddStrength(double strength) {
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
			double bestStrength = -100.0;

			// Next we need to find which strength is closest to the average:
			foreach (var strength in Data) {
				double diff = Math.Abs(average - strength);

				if (bestStrength == -100.0 || diff < bestDiff) {
					bestStrength = strength;
					bestDiff = diff;
				}
			}

			return bestStrength;
		}
	}
}