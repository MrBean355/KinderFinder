using System;
using System.Collections.Generic;
using System.IO;

using Android.Graphics;

namespace KinderFinder.Utility {

	public class MapCache {
		static readonly string APP_FOLDER = Android.OS.Environment.ExternalStorageDirectory + "/" + Settings.Storage.BASE;

		readonly string CacheFilePath;

		public MapCache(string filePath = "cache") {
			CacheFilePath = filePath;

			// Create the "KinderFinder/" folder if it doesn't exist:
			if (!Directory.Exists(APP_FOLDER)) {
				Directory.CreateDirectory(APP_FOLDER);
				Console.WriteLine("Info: Created app folder.");
			}

			// Create the "KinderFinder/maps/" folder if it doesn't exist:
			if (!Directory.Exists(APP_FOLDER + Settings.Storage.MAPS)) {
				Directory.CreateDirectory(APP_FOLDER + Settings.Storage.MAPS);
				Console.WriteLine("Info: Created maps folder.");
			}

			// Create the cache file if it doesn't exist:
			if (!File.Exists(APP_FOLDER + CacheFilePath)) {
				File.Create(APP_FOLDER + "temp").Close();
				SharpAESCrypt.SharpAESCrypt.Encrypt(Settings.ENCRYPTION_PASSWORD, APP_FOLDER + "temp", APP_FOLDER + CacheFilePath);
				File.Delete(APP_FOLDER + "temp");
				Console.WriteLine("Info: Created cache file.");
			}
		}

		/// <summary>
		/// Decrypts the encrypted cache file and reads the text.
		/// </summary>
		/// <returns>The decrypted version of the cache file.</returns>
		string[] ReadEncryptedFile() {
			SharpAESCrypt.SharpAESCrypt.Decrypt(Settings.ENCRYPTION_PASSWORD, APP_FOLDER + CacheFilePath, APP_FOLDER + "temp");
			string[] result = File.ReadAllLines(APP_FOLDER + "temp");
			File.Delete(APP_FOLDER + "temp");

			return result;
		}

		/// <summary>
		/// Rewrites the encrypted cache file, replacing all existing content.
		/// </summary>
		/// <param name="lines">Lines to write to the cache file.</param>
		void WriteEncryptedFile(List<string> lines) {
			using (var writer = new StreamWriter(APP_FOLDER + "temp", false)) {
				foreach (string line in lines)
					writer.WriteLine(line);
			}

			SharpAESCrypt.SharpAESCrypt.Encrypt(Settings.ENCRYPTION_PASSWORD, APP_FOLDER + "temp", APP_FOLDER + CacheFilePath);
			File.Delete(APP_FOLDER + "temp");
		}

		/// <summary>
		/// Determines whether the locally stored version of a restautant's map matches the server's version.
		/// </summary>
		/// <param name="restName">Restaurant name.</param>
		/// <param name="bytesCount">Server's number of bytes of map.</param>
		/// <returns>True if maps are the same; false otherwise.</returns>
		public bool IsMapSame(string restName, int bytesCount) {
			string[] lines = ReadEncryptedFile();

			foreach (string line in lines) {
				string[] a = line.Split(':');

				if (a.Length > 1) {
					if (a[0].Equals(restName, StringComparison.CurrentCultureIgnoreCase)) {
						int parsed;

						if (int.TryParse(a[1], out parsed))
							return parsed == bytesCount;
					}
				}
				else {
					Console.WriteLine("Warning: Possible corruption of cache file '" + CacheFilePath + "'.");
					return false;
				}
			}

			return false;
		}

		/// <summary>
		/// Adds a restaurant's map to the cache file. Updates the cache if the map is already there.
		/// </summary>
		/// <param name="restName">Restaurant name.</param>
		/// <param name="bytesCount">Number of bytes of the map.</param>
		/// <param name="map">Bitmap representation of the map.</param>
		public void AddMap(string restName, int bytesCount, Bitmap map) {
			string[] lines = ReadEncryptedFile();
			var result = new List<string>();
			bool found = false;

			// For each line in the cache file:
			foreach (string line in lines) {
				string[] a = line.Split(':');

				// Entry is valid:
				if (a.Length > 1) {
					// This entry belongs to the desired restaurant:
					if (a[0].Equals(restName, StringComparison.CurrentCultureIgnoreCase)) {
						result.Add(restName + ":" + bytesCount);
						found = true;

						// Store map image on device:
						using (var stream = new FileStream(APP_FOLDER + Settings.Storage.MAPS + restName + Settings.Storage.IMAGE_EXTENSION, FileMode.Create))
							map.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);

						Console.WriteLine("Info: Updated '" + restName + "' in cache file.");
					}
					// This entry belongs to another restaurant; just add it to output:
					else
						result.Add(line);
				}
				// Entry is invalid; don't add it to output:
				else
					Console.WriteLine("Warning: Possible corruption of cache file '" + CacheFilePath + "'.\n\t-- Misbehaving line: '" + line + "'.\n\t----This line has been removed.");
			}

			// Map not found in file; add entry:
			if (!found) {
				result.Add(restName + ":" + bytesCount);

				using (var stream = new FileStream(APP_FOLDER + Settings.Storage.MAPS + restName + Settings.Storage.IMAGE_EXTENSION, FileMode.Create))
					map.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);

				Console.WriteLine("Info: Added '" + restName + "' to cache file.");
			}

			// Update cache file:
			WriteEncryptedFile(result);
		}

		/// <summary>
		/// Gets the locally stored map of a restaurant.
		/// </summary>
		/// <param name="restName">Restaurant name.</param>
		/// <returns>The stored map if found; null otherwise.</returns>
		public Bitmap GetStoredMap(string restName) {
			string path = APP_FOLDER + Settings.Storage.MAPS + restName + Settings.Storage.IMAGE_EXTENSION;

			if (!File.Exists(path))
				return null;

			return BitmapFactory.DecodeFile(path);
		}
	}
}

