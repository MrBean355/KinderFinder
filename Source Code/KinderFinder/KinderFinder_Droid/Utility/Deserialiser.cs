using System.IO;
using System.Runtime.Serialization.Json;

namespace KinderFinder.Utility {

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
}
