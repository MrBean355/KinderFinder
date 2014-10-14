using System;
using System.IO;
using System.Net;

namespace Transmitter.Utility {
		
	public static class AppTools {

		public static ServerResponse SendRequest(string url, string json, int timeout = 10000) {
			var result = new ServerResponse();
			result.StatusCode = HttpStatusCode.OK;

			var req = WebRequest.Create("http://192.168.1.7:55555/" + url) as HttpWebRequest;
			req.ContentType = "application/json";
			req.Method = "POST";
			req.Timeout = timeout;

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

				/* Read the response message's body. */
				using (var reader = new StreamReader(response.GetResponseStream())) {
					result.Body = reader.ReadToEnd();
					reader.Close();
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
			}

			return result;
		}

		public struct ServerResponse {
			public HttpStatusCode StatusCode;
			public string Body;
		}
	}
}

