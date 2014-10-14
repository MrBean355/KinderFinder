using Java.Lang;
using Java.Util;

namespace no.nordicsemi.android.beacon {

	/**
 * The iBeacon region class.
 */
	public class BeaconRegion {
		public static UUID ANY_UUID = null;
		public static int ANY = -1;

		private UUID mUuid;
		private int mMajor;
		private int mMinor;

		/**
	 * Constructs the region object
	 * 
	 * @param uuid
	 *            the region UUID
	 * @param major
	 *            the major number (16-bit, unsigned int)
	 * @param minor
	 *            the minor number (16-bit, unsigned int)
	 */
		public BeaconRegion(UUID uuid, int major, int minor) {
			mUuid = uuid;
			mMajor = major;
			mMinor = minor;
		}

		/**
	 * Returns the region UUID as it was registered eariler. May be equal to {@link #ANY_UUID}.
	 * 
	 * @return the region UUID
	 */
		public UUID getUuid() {
			return mUuid;
		}

		/**
	 * The major beacon number as registered before. May be equal to {@link #ANY}.
	 * 
	 * @return the major beacon number (16-bit, unsigned int)
	 */
		public int getMajor() {
			return mMajor;
		}

		/**
	 * The minor beacon number as registered before. May be equal to {@link #ANY}.
	 * 
	 * @return the minor beacon number (16-bit, unsigned int)
	 */
		public int getMinor() {
			return mMinor;
		}

		public override string ToString() {
			return mUuid + " major: " + mMajor + " minor: " + mMinor;
		}

		public override bool Equals(System.Object o) {
			try {
				BeaconRegion other = (BeaconRegion)o;
				return mUuid.Equals(other.mUuid) && mMajor == other.mMajor && mMinor == other.mMinor;
			}
			catch (ClassCastException e) {
			}
			return false;
		}
	}
}
