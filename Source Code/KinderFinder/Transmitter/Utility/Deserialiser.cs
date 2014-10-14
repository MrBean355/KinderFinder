using System.IO;
using System.Runtime.Serialization.Json;

namespace Transmitter.Utility {

	public static class Deserialiser<T> {

		/// <summary>
		/// Deserialises a JSON string to the specified type.
		/// </summary>
		/// <param name="json">Json.</param>
		public static T Run(string json) {
			var ser = new DataContractJsonSerializer(typeof(T));
			var ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(json));
			var result = (T)ser.ReadObject(ms);

			ms.Close();
			return result;
		}
	}

	public static class Serialiser<T> {

		public static string Run(T input) {
			var ser = new DataContractJsonSerializer(typeof(T));
			var ms = new MemoryStream();
			ser.WriteObject(ms, input);
			ms.Position = 0;
			var reader = new StreamReader(ms);
			string result = reader.ReadToEnd();
			reader.Close();

			return result;
		}
	}
}
