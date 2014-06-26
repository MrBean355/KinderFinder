using System;
using System.Linq;

namespace KinderFinder {

	class Program {

		/**
		 * Simple main function that tests the ORM.
		 * To get Entity Framework working in Visual Studio, follow the
		 * instructions here:
		 *     http://msdn.microsoft.com/en-us/data/jj200620
		 * Run the SQL script first, to create the database.
		 */
		static void Main(string[] args) {
			using (var db = new KinderFinderEntities()) {
				// List all existing restaurants:
				Console.WriteLine("Initial restaurants:");
				var query = from item in db.Restaurants
							orderby item.Name
							select item;

				foreach (Restaurant rest in query)
					Console.WriteLine(rest.Name);

				// Add a new restaurant:
				Restaurant wimpy = new Restaurant();
				// Not needed to set its ID, because it is a primary key and is
				// auto-incremented.
				wimpy.Name = "Wimpy";

				db.Restaurants.Add(wimpy);
				db.SaveChanges();

				// List all existing restaurants:
				Console.WriteLine("\nRestaurants after adding:");
				query = from item in db.Restaurants
						orderby item.Name
						select item;

				foreach (Restaurant rest in query)
					Console.WriteLine(rest.Name);

				// Delete the Wimpy restaurant:
				wimpy = db.Restaurants.First(i => i.Name == "Wimpy");
				db.Restaurants.Remove(wimpy);
				db.SaveChanges();

				// List all existing restaurants:
				Console.WriteLine("\nRestaurants after removing:");
				query = from item in db.Restaurants
						orderby item.Name
						select item;

				foreach (Restaurant rest in query)
					Console.WriteLine(rest.Name);
			}

			Console.WriteLine("\nPress any key to continue...");
			Console.ReadKey();
		}
	}
}
