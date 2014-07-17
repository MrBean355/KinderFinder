using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;

namespace KinderFinder_Droid {

	public static class Utility {
		private const string SERVER = "http://192.168.1.7:55555/";
		private const string SALT_VALUE = "2e6e76485b61254b2e73694d50";

		public static bool IsValidEmailAddress(string email) {
			Regex regex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$");
			return regex.IsMatch(email);
		}

		/**
		 * Attempts to send data, in the form of a JSON string, to the server
		 * using the POST method.
		 * 
		 * @param url URL after server's address (e.g. "api/login").
		 * @param json Data to send, in the form of a JSON object.
		 * @returns Response from the server if successful; null otherwise.
		 */
		public static HttpStatusCode SendData(string url, string json) {
			try {
				var req = WebRequest.Create(SERVER + url) as HttpWebRequest;
				req.ContentType = "application/json";
				req.Method = "POST";

				using (var writer = new StreamWriter(req.GetRequestStream())) {
					writer.Write(json);
					writer.Flush();
					writer.Close();

					var response = (HttpWebResponse)req.GetResponse();
					// TODO: Check received data.
					/*using (var reader = new StreamReader(response.GetResponseStream())) {
						result.statusCode = response.StatusCode.ToString();
						result.body = reader.ReadToEnd();
						reader.Close();
					}*/
				}
			}
			catch (WebException ex) {
				HttpWebResponse r = (HttpWebResponse)ex.Response;
				return r.StatusCode;
			}
			catch (Exception ex) {
				return HttpStatusCode.ExpectationFailed;
			}

			return HttpStatusCode.OK;
		}

		public static string HashPassword(string password) {
			// merge password and salt together
			string sHashWithSalt = password + SALT_VALUE;
			// convert this merged value to a byte array
			byte[] saltedHashBytes = Encoding.UTF8.GetBytes(sHashWithSalt);
			// use hash algorithm to compute the hash
			System.Security.Cryptography.HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed();
			// convert merged bytes to a hash as byte array
			byte[] hash = algorithm.ComputeHash(saltedHashBytes);
			// return the has as a base 64 encoded string
			return Convert.ToBase64String(hash);
		}
	}
}

