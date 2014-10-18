
namespace KinderFinder.Utility {

	public static class Settings {
		// TODO: Replace static with const.
		public static string SERVER_ADDRESS = "http://192.168.1.7:55555/";
		public const int REQUEST_TIMEOUT = 10000;

		public const string PREFERENCES_FILE = "Preferences";

		public const string HASHING_SALT = "2e6e76485b61254b2e73694d50";
		public const string ENCRYPTION_PASSWORD = "fv651J5C38v7P7u76W5a5Ely9mk8PM6A";

		public static class Storage {
			public const string BASE = "KinderFinder/";
			public const string MAPS = "maps/";
			public const string IMAGE_EXTENSION = ".jpg";
		}

		public static class Errors {
			public const string SERVER_ERROR = "Server error. Please try again later";
			public const string LOCAL_DATA_ERROR = "Unable to load local data. Please log in again";
		}

		public static class Keys {
			public const string USERNAME = "username";
			public const string PASSWORD_HASH = "passwordhash";
			public const string REMEMBER_ME = "rememberme";
			public const string RESTAURANT_NAME = "currentrestaurant";
			public const string TAG_NAME = "_name";
			public const string TAG_COLOUR = "_colour";
			public const string CURRENT_TAG = "currenttag";
		}

		public static class Lengths {
			public const int NAME_MIN = 3;
			public const int NAME_MAX = 50;
			public const int PASSWORD_MIN = 6;
			public const int PASSWORD_MAX = 50;
		}

		public static class Map {
			public const string DEFAULT_DOT_COLOUR = "00FF00";
			public const string PROBLEM_DOT_COLOUR = "FF0000";
			public const int DOT_COLOUR_ALPHA = 150;
			public const int DOT_SIZE_RADIUS = 10;

			public const int UPDATE_FREQUENCY = 1000;
			public const float OVERLAY_TEXT_SIZE = 40.0f;
			public const string UNKNOWN_NAME_TEXT = "Unknown";
			public const int MAX_ZOOMS = 5;
			public const double ZOOM_AMOUNT = 0.1;
		}

		public static class SpecialPoints {
			public const double OUT_OF_RANGE = -100.0;
		}
	}
}
