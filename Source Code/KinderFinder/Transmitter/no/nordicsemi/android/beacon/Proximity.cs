namespace no.nordicsemi.android.beacon {

	/**
 * The enumeration with beacon proximity values.
 */
	public enum Proximity {
		/** The proximity of the beacon could not be determined. The signal has not been received in 1600 milliseconds or the received RSSI value was 0. */
		UNKNOWN,
		/** The beacon is in the user’s immediate vicinity. */
		IMMEDIATE,
		/** The beacon is relatively close to the user. */
		NEAR,
		/** The beacon is far away. */
		FAR
	}
}