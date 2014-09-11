using System;

namespace Locator {

	class Program {

		static void Main(string[] args) {
			Trilaterator t = new Trilaterator();

			t.AddNode(0.0, 0.0);
			t.AddNode(450.0, 0.0);
			t.AddNode(450.0, 450.0);
			t.MoveTag(120.0, 80.0);

			var pos = t.Locate();

			t.Print();
			Console.WriteLine("\nLocation: (" + pos[0] + ", " + pos[1] + ")");

			Console.WriteLine("\nPress any key to exit...");
			Console.ReadKey();
		}
	}
}
