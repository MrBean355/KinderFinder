using System.Text.RegularExpressions;

namespace KinderFinder {

	public static class Validator {

		/// <summary>
		/// Determines if a string is a valid email address.
		/// </summary>
		/// <param name="input">Input string to check.</param>
		/// <returns>True if it's a valid email address; false otherwise.</returns>
		public static bool IsValidEmailAddress(string input) {
			var regex = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$");
			return regex.IsMatch(input);
		}

		/// <summary>
		/// Determines if a string is a valid email name. Does not work for international names (i.e. ones containing
		/// non-English characters.
		/// </summary>
		/// <param name="input">Input string to check.</param>
		/// <returns>True if it's a valid name; false otherwise.</returns>
		public static bool IsValidName(string input) {
			string length = Settings.Lengths.NAME_MIN + "," + Settings.Lengths.NAME_MAX;
			var regex = new Regex("^[a-zA-Z ,.'-]{" + length + "}$");
			return regex.IsMatch(input);
		}

		/// <summary>
		/// Determines if a string is a valid password. Passwords can contain any characters but must be a certain
		/// length.
		/// </summary>
		/// <param name="input">Input string to check.</param>
		/// <returns>True if it's a valid password; false otherwise.</returns>
		public static bool IsValidPassword(string input) {
			return input.Length >= Settings.Lengths.PASSWORD_MIN && input.Length <= Settings.Lengths.PASSWORD_MAX;
		}
	}
}
