
namespace KinderFinder_Droid {

	public static class Globals {
		/// <summary>
		/// Name of the shared preferences file, used for persistent storage.
		/// </summary>
		public const string PREFERENCES_FILE = "Preferences";

		/// <summary>
		/// Key for username storage in the preferences file.
		/// </summary>
		public const string KEY_USERNAME = "username";
		public const string KEY_PASSWORD_HASH = "passwordhash";
		public const string KEY_REMEMBER_ME = "rememberme";
		public const string KEY_RESTAURANT_NAME = "currentrestaurant";
		//public const string KEY_MAP_SIZE = "mapbyteslength";

		/// <summary>
		/// Minimum length that names can be.
		/// </summary>
		public const int NAME_MIN_LENGTH = 3;
		/// <summary>
		/// Maximum length that names can be.
		/// </summary>
		public const int NAME_MAX_LENGTH = 50;
		/// <summary>
		/// Minimum length that passwords can be.
		/// </summary>
		public const int PASSWORD_MIN_LENGTH = 6;
		/// <summary>
		/// Maximum length that passwords can be.
		/// </summary>
		public const int PASSWORD_MAX_LENGTH = 50;
	}
}

