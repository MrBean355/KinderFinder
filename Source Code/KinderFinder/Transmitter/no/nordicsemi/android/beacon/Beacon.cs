using Java.Util;

namespace no.nordicsemi.android.beacon {

	/**
 * The Beacon object.
 */
	public class Beacon {
		private string mAddress;
		public UUID uuid;
		public int major;
		public int minor;
		public int rssi;
		public float accuracy;
		public float previousAccuracy;

		/**
	 * Constructs the beacon object.
	 * 
	 * @param address
	 *            the beacon device address
	 */
		public Beacon(string address) {
			mAddress = address;
			accuracy = -1.0f;
			previousAccuracy = -1.0f;
		}

		/**
	 * Returns the beacon's device address.
	 * 
	 * @return the device address
	 */
		public string getDeviceAddress() {
			return mAddress;
		}

		/**
	 * Returns the beacon service 128-bit UUID.
	 * 
	 * @return the UUID
	 */
		public UUID getUuid() {
			return uuid;
		}

		/**
	 * Returns the beacon 16-bit major number.
	 * 
	 * @return the major number
	 */
		public int getMajor() {
			return major;
		}

		/**
	 * Returns the beacon 16-bit minor number.
	 * 
	 * @return the minor number
	 */
		public int getMinor() {
			return minor;
		}

		/**
	 * Returns the average RSSI value of last 10 samples received.
	 * 
	 * @return The received signal strength of the beacon, measured in mDb.
	 */
		public int getRssi() {
			return rssi;
		}

		/**
	 * Returns the estimated relative distance to the beacon. Use it to quickly identify beacons that are nearer to the user rather than farther away.
	 * 
	 * @return The relative distance to the beacon.
	 */
		public Proximity getProximity() {
			return calculateProximity(this.accuracy);
		}

		/**
	 * Returns the estimated relative distance to the beacon a second ago. Use it to quickly check whether you are getting closer or moving away from the beacon.
	 * 
	 * @return The previous relative distance to the beacon.
	 */
		public Proximity getPreviousProximity() {
			return calculateProximity(this.previousAccuracy);
		}

		/**
	 * The accuracy of the proximity value, measured in meters from the beacon. Use this property to differentiate between beacons with the same proximity value. Do not use it to identify a precise
	 * location for the beacon. Accuracy values may fluctuate due to RF interference.
	 * 
	 * @return the approximate distance to the beacon in meters
	 */
		public float getAccuracy() {
			return accuracy;
		}

		public override string ToString() {
			return uuid + " major: " + major + " minor: " + minor + " | proximity: " + getProximity() + " accuracy: " + accuracy + "m";
		}

		/**
	 * Calculates the proximity value basing on the accuracy.
	 * 
	 * @param accuracy
	 *            the approximate distance to the beacon in meters
	 * @return the relative distance to the beacon.
	 */
		private Proximity calculateProximity(float accuracy) {
			if (accuracy == -1.0f) {
				return Proximity.UNKNOWN;
			}
			if (accuracy <= 0.26) {
				return Proximity.IMMEDIATE;
			}
			if (accuracy <= 2.0) {
				return Proximity.NEAR;
			}
			return Proximity.FAR;
		}
	}

}