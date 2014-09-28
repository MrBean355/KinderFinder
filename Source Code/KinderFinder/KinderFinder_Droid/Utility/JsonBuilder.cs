using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace KinderFinder {

	public class JsonBuilder {
		List<KeyValuePair<string, string>> Entries = new List<KeyValuePair<string, string>>();

		public void AddEntry(string key, string value) {
			Entries.Add(new KeyValuePair<string, string>(key, value));
		}

		public override string ToString() {
			string result = "{";

			foreach (var entry in Entries) {
				result += "\"" + entry.Key + "\":";
				result += "\"" + entry.Value + "\",";
			}

			if (result[result.Length - 1] == ',')
				result = result.Remove(result.Length - 1, 1);

			result += '}';

			return result;
		}
	}

	static class Deserialiser<T> {

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
}
	