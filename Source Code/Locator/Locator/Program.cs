using System;

namespace Locator {

	class Program {

		static void Main(string[] args) {
			Trilaterator t = new Trilaterator();

			t.AddNode(0.0, 0.0);
			t.AddNode(0.87, 0.0);
			t.AddNode(0.87, 0.87);

			//t.MoveTag(120.0, 80.0);

			//var pos = t.Locate();
			var pos = t.Run(-0.3, -1.125, -0.409);

			//t.Print();
			Console.WriteLine("\nLocation: (" + pos[0] + ", " + pos[1] + ")");

			Console.WriteLine("\nPress any key to exit...");
			Console.ReadKey();
		}
	}
}
