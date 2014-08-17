using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Android.Widget;

namespace KinderFinder_Droid {

	/**
	 * Represents a simple response from a server; containing a status code and message
	 * body.
	 */
	public struct ServerResponse {
		public HttpStatusCode StatusCode;
		public string Body;
		public byte[] Bytes;
	}

	public static class Utility {
		const string SERVER = "http://192.168.1.7:55555/";
		const string SALT_VALUE = "2e6e76485b61254b2e73694d50";

		public static List<string> ParseJSON(string json) {
			var result = new List<string>();

			if (json.Length == 0)
				return result;

			/* Remove '[' and ']' from string. */
			if (json[0] == '[') {
				json = json.Remove(json.Length - 1, 1);
				json = json.Remove(0, 1);
			}

			if (json.Length == 0)
				return result;

			string[] a = json.Split(',');

			foreach (string s in a) {
				string temp = s;

				/* Remove surrounding quotation marks. */
				if (temp[0] == '"' && temp[temp.Length - 1] == '"') {
					temp = s.Remove(s.Length - 1, 1);
					temp = temp.Remove(0, 1);
				}

				result.Add(temp);
			}

			return result;
		}

		/**
		 * Determines whether a string is a valid email address or not.
		 * 
		 * @param email Email address to check.
		 * @returns True if valid; false otherwise.
		 */
		public static bool IsValidEmailAddress(string email) {
			var regex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$");
			return regex.IsMatch(email);
		}

		/**
		 * Attempts to send data, in the form of a JSON string, to the server
		 * using the POST method.
		 * 
		 * @param url URL after server's address (e.g. "api/login").
		 * @param json Data to send, in the form of a JSON object.
		 * @returns Response from the server.
		 */
		public static ServerResponse SendData(string url, string json) {
			var result = new ServerResponse();
			result.StatusCode = HttpStatusCode.OK;

			try {
				var req = WebRequest.Create(SERVER + url) as HttpWebRequest;
				req.ContentType = "application/json";
				req.Method = "POST";

				if (json == null)
					json = "";

				/* Write the message's body. */
				using (var writer = new StreamWriter(req.GetRequestStream())) {
					writer.Write(json);
					writer.Flush();
					writer.Close();
				}

				var response = (HttpWebResponse)req.GetResponse();
				Stream input = response.GetResponseStream();

				/* If the response is an image, convert it to a byte[]. */
				if (response.ContentType == "image/jpg") {
					result.Bytes = new byte[input.Length];
					int index = 0;
					int read;

					/* Read until we get to the end. */
					while ((read = input.ReadByte()) != -1)
						result.Bytes[index++] = (byte)read;
				}
				/* Otherwise store it as a string. */
				else {
					/* Read the response message's body. */
					using (var reader = new StreamReader(response.GetResponseStream())) {
						result.Body = reader.ReadToEnd();
						reader.Close();
					}
				}
			}
			/* Status code other than the 200s was returned. */
			catch (WebException ex) {
				var response = (HttpWebResponse)ex.Response;

				if (response == null)
					Environment.Exit(-1);

				result.StatusCode = response.StatusCode;
			}
			/* Something else broke. */
			catch (Exception ex) {
				result.StatusCode = HttpStatusCode.ServiceUnavailable;
				Console.WriteLine("EXCEPTION while communication with server:\n" + ex);
				Toast.MakeText(null, "Unable to contact server", ToastLength.Long);
			}

			return result;
		}

		/**
		 * Hashes a password.
		 * Source: http://www.devtoolshed.com/c-generate-password-hash-salt
		 * 
		 * @param password Password to hash.
		 * @returns Hashed password.
		 */
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

