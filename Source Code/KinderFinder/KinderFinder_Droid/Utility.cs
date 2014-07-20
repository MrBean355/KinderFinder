using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace KinderFinder_Droid {

	/**
	 * Represents a simple response from a server; containing a status code and message
	 * body.
	 */
	public struct ServerResponse {
		public HttpStatusCode StatusCode;
		public string Body;
	}

	public static class Utility {
		const string SERVER = "http://192.168.1.7:55555/";
		const string SALT_VALUE = "2e6e76485b61254b2e73694d50";

		public static List<string> ParseJSON(string json) {
			var result = new List<string>();

			/* Remove '[' and ']' from string. */
			json = json.Remove(json.Length - 1, 1);
			json = json.Remove(0, 1);

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
			Regex regex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$");
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
			ServerResponse result = new ServerResponse();
			result.StatusCode = HttpStatusCode.OK;

			try {
				var req = WebRequest.Create(SERVER + url) as HttpWebRequest;
				req.ContentType = "application/json";
				req.Method = "POST";

				/* Write the message's body. */
				using (var writer = new StreamWriter(req.GetRequestStream())) {
					writer.Write(json);
					writer.Flush();
					writer.Close();
				}

				var response = (HttpWebResponse)req.GetResponse();

				/* Read the response message's body. */
				using (var reader = new StreamReader(response.GetResponseStream())) {
					result.Body = reader.ReadToEnd();
					reader.Close();
				}
			}
			/* Status code other than the 200s was returned. */
			catch (WebException ex) {
				var response = (HttpWebResponse)ex.Response;
				result.StatusCode = response.StatusCode;
			}
			/* Something else broke. */
			catch (Exception ex) {
				result.StatusCode = HttpStatusCode.ServiceUnavailable;
				Console.WriteLine("EXCEPTION while communication with server:\n" + ex);
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

