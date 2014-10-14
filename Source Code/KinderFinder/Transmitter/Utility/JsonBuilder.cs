using System.Collections.Generic;

namespace Transmitter.Utility {

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
}
	