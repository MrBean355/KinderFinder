using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Android.Graphics;
using Android.Widget;

namespace KinderFinder {

	public static class AppTools {

		/// <summary>
		/// Attempts to parse a JSON string into a list.
		/// </summary>
		/// <param name="json">Input string to parse./param>
		/// <returns>List version of the input.</returns>
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

		/// <summary>
		/// Attempts to send data in the form of a JSON string to the server using the POST method.
		/// </summary>
		/// <param name="url">URL to send the request to.</param>
		/// <param name="json">JSON string to send. Can be null.</param>
		/// <returns>Response from the server.</returns>
		public static ServerResponse SendRequest(string url, string json) {
			var result = new ServerResponse();
			result.StatusCode = HttpStatusCode.OK;

			var req = WebRequest.Create(Settings.SERVER_ADDRESS + url) as HttpWebRequest;
			req.ContentType = "application/json";
			req.Method = "POST";
			req.Timeout = Settings.REQUEST_TIMEOUT;

			if (json == null)
				json = "";

			try {
				/* Write the message's body. */
				using (var writer = new StreamWriter(req.GetRequestStream())) {
					writer.Write(json);
					writer.Flush();
					writer.Close();
				}

				var response = (HttpWebResponse)req.GetResponse();

				/* If the response is an image, convert it to a byte[]. */
				if (response.ContentType == "image/jpg") {
					var responseStream = response.GetResponseStream();
					result.Bytes = new byte[responseStream.Length];
					int index = 0;
					int read;

					/* Read until we get to the end. */
					while ((read = responseStream.ReadByte()) != -1)
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

				/* The response will be null if the request timed out. */
				if (response == null)
					result.StatusCode = HttpStatusCode.RequestTimeout;
				else
					result.StatusCode = response.StatusCode;
			}
			/* Another exception was thrown in this method. */
			catch (Exception ex) {
				result.StatusCode = HttpStatusCode.ServiceUnavailable;
				Console.WriteLine("EXCEPTION while communication with server:\n" + ex);
				Toast.MakeText(null, "Unable to contact server", ToastLength.Long);
			}

			return result;
		}

		/// <summary>
		/// Hashes a password.
		/// </summary>
		/// <param name="password">Password to hash.</param>
		/// <returns>Hash of the provided password.</returns>
		public static string HashPassword(string password) {
			// merge password and salt together
			string sHashWithSalt = password + Settings.HASHING_SALT;
			// convert this merged value to a byte array
			byte[] saltedHashBytes = Encoding.UTF8.GetBytes(sHashWithSalt);
			// use hash algorithm to compute the hash
			System.Security.Cryptography.HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed();
			// convert merged bytes to a hash as byte array
			byte[] hash = algorithm.ComputeHash(saltedHashBytes);
			// return the has as a base 64 encoded string
			return Convert.ToBase64String(hash);
		}

		/// <summary>
		/// Resizes a bitmap to a new width and height, retaining the image's scale.
		/// </summary>
		/// <returns>Resized bitmap.</returns>
		/// <param name="input">Bitmap to resize.</param>
		/// <param name="targWidth">Target width.</param>
		/// <param name="targHeight">Target height.</param>
		public static Bitmap ResizeBitmap(Bitmap input, int targWidth, int targHeight) {
			double ratioW = (double)targWidth / input.Width;
			double ratioH = (double)targHeight / input.Height;
			int newWidth, newHeight;

			if (ratioW <= ratioH) {
				newWidth = (int)(input.Width * ratioW);
				newHeight = (int)(input.Height * ratioW);
			}
			else {
				newWidth = (int)(input.Width * ratioH);
				newHeight = (int)(input.Height * ratioH);
			}

			return Bitmap.CreateScaledBitmap(input, newWidth, newHeight, false);
		}
	}

	/// <summary>
	/// Represents a response from the server. Contains a status code, message body and an array of bytes (which can be
	/// used to store an image.
	/// </summary>
	public struct ServerResponse {
		public HttpStatusCode StatusCode;
		public string Body;
		public byte[] Bytes;
	}
}

