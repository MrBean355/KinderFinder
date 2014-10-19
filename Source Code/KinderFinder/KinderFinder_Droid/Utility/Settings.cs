
namespace KinderFinder.Utility {

	public static class Settings {

		public static class Network {
			/// <summary>
			/// IP address of the server to connect to, including the port number.
			/// </summary>
			public const string SERVER_ADDRESS = "http://192.168.1.7:55555/";
			/// <summary>
			/// Timeout, in milliseconds, of all requests sent.
			/// </summary>
			public const int REQUEST_TIMEOUT = 10000;
			/// <summary>
			/// How often the map is updated (requests new locations).
			/// </summary>
			public const int UPDATE_FREQUENCY = 1000;
		}

		public static class Encryption {
			/// <summary>
			/// Salt value to use.
			/// </summary>
			public const string SALT = "2e6e76485b61254b2e73694d50";
			/// <summary>
			/// Password for encryption and decryption.
			/// </summary>
			public const string PASSWORD = "fv651J5C38v7P7u76W5a5Ely9mk8PM6A";
		}

		public static class Storage {
			/// <summary>
			/// Name of the preferences file to use.
			/// </summary>
			public const string PREFERENCES_FILE = "Preferences";
			/// <summary>
			/// Base folder name for storage on the device.
			/// </summary>
			public const string BASE = "KinderFinder/";
			/// <summary>
			/// Folder to store cached maps in.
			/// </summary>
			public const string MAPS = "maps/";
			/// <summary>
			/// Format to store images in.
			/// </summary>
			public const string IMAGE_EXTENSION = ".jpg";
		}

		public static class Errors {
			/// <summary>
			/// Generic text to display when the server couldn't be reached or gave an error.
			/// </summary>
			public const string SERVER_ERROR = "Server error. Please try again later";
			/// <summary>
			/// Text to display when data in the preferences file couldn't be loaded.
			/// </summary>
			public const string LOCAL_DATA_ERROR = "Unable to load local data. Please log in again";
		}

		/**
		 * Various keys to use when accessing the preferences file.
		 */
		public static class Keys {
			public const string USERNAME = "username";
			public const string PASSWORD_HASH = "passwordhash";
			public const string REMEMBER_ME = "rememberme";
			public const string RESTAURANT_NAME = "currentrestaurant";
			public const string TAG_NAME = "_name";
			public const string TAG_COLOUR = "_colour";
			public const string CURRENT_TAG = "currenttag";
		}

		/**
		 * Various text length restrictions for user input.
		 */
		public static class Lengths {
			public const int NAME_MIN = 3;
			public const int NAME_MAX = 50;
			public const int PASSWORD_MIN = 6;
			public const int PASSWORD_MAX = 50;
		}

		public static class Map {
			/// <summary>
			/// Default tracking dot colour (when it hasn't been customised).
			/// </summary>
			public const string DEFAULT_DOT_COLOUR = "00FF00";
			/// <summary>
			/// Colour to make the tracking dot when the tag is out of range.
			/// </summary>
			public const string PROBLEM_DOT_COLOUR = "FF0000";
			/// <summary>
			/// Tracking dot opacity (between 0 and 255).
			/// </summary>
			public const int DOT_COLOUR_ALPHA = 150;
			/// <summary>
			/// Size of the tracking dots.
			/// </summary>
			public const int DOT_SIZE_RADIUS = 10;
			/// <summary>
			/// Font size of names overlayed onto the map.
			/// </summary>
			public const float OVERLAY_TEXT_SIZE = 40.0f;
			/// <summary>
			/// Default name to display when one couldn't be loaded (hasn't been set).
			/// </summary>
			public const string UNKNOWN_NAME_TEXT = "Unknown";
		}

		public static class SpecialPoints {
			/// <summary>
			/// Special value indicating a tag is out of range.
			/// </summary>
			public const double OUT_OF_RANGE = -100.0;
			/// <summary>
			/// Special value indicating there is a problem with the system (a transmitter isn't sending a strength
			/// to a tag.
			/// </summary>
			public const double TRANSMITTER_PROBLEM = -50.0;
		}
	}
}
