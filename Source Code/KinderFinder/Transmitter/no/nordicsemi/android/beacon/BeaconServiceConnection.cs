using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Util;

using Java.Lang;
using Java.Util;

namespace no.nordicsemi.android.beacon {

	/**
	 * The listener for ranging the beacons events. After registration it will receive an event every second with a (possibly empty) list of beacons matching the region.
	 */
	public interface BeaconsListener {
		/**
		 * This callback method is being called every second after starting ranging for beacons. It's called even if no beacons are in range giving the empty beacons array.
		 * 
		 * @param beacons
		 *            the array with beacons in the range. This is never <code>null</code> but may be empty.
		 * @param region
		 *            the region in where the beacons were found.
		 */
		void OnBeaconsInRegion(Beacon[] beacons, BeaconRegion region);
	}

	/**
	 * The listener for active region monitoring events.
	 */
	public interface RegionListener {
		/**
		 * Called when user has entered the registered region and at least one beacon matching the region parameters is in range.
		 * 
		 * @param region
		 *            The region registered with {@link BeaconServiceConnection#startMonitoringForRegion(UUID, int, int, RegionListener)}.
		 */
		void OnEnterRegion(BeaconRegion region);

		/**
		 * Called when user has exited the registered region and no beacon matching the region parameters is in range for some time.
		 * 
		 * @param region
		 *            The region registered with {@link BeaconServiceConnection#startMonitoringForRegion(UUID, int, int, RegionListener)}.
		 */
		void OnExitRegion(BeaconRegion region);
	}

	/**
 * The service connection used to communicate with Beacon Service from the "nRF Beacon Service" application.
 */
	public abstract class BeaconServiceConnection : Object, IServiceConnection {
		private SparseArray<BeaconRegion> mRegionsByHash = new SparseArray<BeaconRegion>();
		private SparseArray<Beacon> mBeaconsByAddressHash = new SparseArray<Beacon>();
		private Dictionary<BeaconsListener, Messenger> mBeaconsMessengers;
		private Dictionary<RegionListener, Messenger> mRegionMessengers;
		/** The messenger for service communication. This is not a <code>null</code> if service is connected. */
		private Messenger mService;

		/**
	 * The ranging for beacons messenger handler.
	 */
		private class BeaconsListenerHandler : Handler {
			private SparseArray<BeaconRegion> mRegionsByHash;
			private SparseArray<Beacon> mBeaconsByAddressHash;
			private BeaconsListener mListener;

			public BeaconsListenerHandler(BeaconsListener listener, SparseArray<BeaconRegion> regionsByHash, SparseArray<Beacon> beaconsByAddressHash) {
				mListener = listener;
				mRegionsByHash = regionsByHash;
				mBeaconsByAddressHash = beaconsByAddressHash;
			}

			public override void HandleMessage(Message msg) {
				switch (msg.What) {
					case ServiceProxy.MSG_BEACONS_IN_REGION:
						{
							UUID regionUuid = msg.Obj != null ? ((ParcelUuid)msg.Obj).Uuid : null;
							int regionMajor = msg.Arg1;
							int regionMinor = msg.Arg2;

							// Get the region reference from a storage or create a new one
							int hash = regionUuid != null ? regionUuid.GetHashCode() ^ (regionMajor << 16) ^ regionMinor : 0;
							BeaconRegion region = mRegionsByHash.Get(hash, new BeaconRegion(regionUuid, regionMajor, regionMinor));
							mRegionsByHash.Put(hash, region);

							// Get the beacons data
							SparseArray<Beacon> beaconsCache = mBeaconsByAddressHash;
							Bundle args = msg.Data;
							int beaconsCount = args.GetInt(ServiceProxy.EXTRA_COUNT);
							Beacon[] beacons = new Beacon[beaconsCount];
							if (beaconsCount > 0) {
								string[] addresses = args.GetStringArray(ServiceProxy.EXTRA_ADDRESSES);
								IParcelable[] uuids = args.GetParcelableArray(ServiceProxy.EXTRA_UUIDS);
								int[] numbers = args.GetIntArray(ServiceProxy.EXTRA_NUMBERS);
								float[] accuracies = args.GetFloatArray(ServiceProxy.EXTRA_ACCURACIES);
								int[] rssis = args.GetIntArray(ServiceProxy.EXTRA_RSSI_VALUES);

								for (int i = 0; i < beaconsCount; ++i) {
									Beacon beacon = beacons[i] = beaconsCache.Get(addresses[i].GetHashCode(), new Beacon(addresses[i]));
									beaconsCache.Put(addresses[i].GetHashCode(), beacon);

									beacon.uuid = ((ParcelUuid)uuids[i]).Uuid;
									beacon.major = numbers[i] >> 16;
									beacon.minor = numbers[i] & 0xFFFF;
									beacon.previousAccuracy = beacon.accuracy;
									beacon.accuracy = accuracies[i];
									beacon.rssi = rssis[i];
								}
							}
							// Notify the listener
							mListener.OnBeaconsInRegion(beacons, region);
							break;
						}
				}
			}
		}

		/**
	 * The region monitoring messenger handler.
	 */
		private class RegionListenerHandler : Handler {
			private SparseArray<BeaconRegion> mRegionsByHash;
			private RegionListener mListener;

			public RegionListenerHandler(RegionListener listener, SparseArray<BeaconRegion> regionsByHash) {
				mListener = listener;
				mRegionsByHash = regionsByHash;
			}

			public void HandleMessage(Message msg) {
				switch (msg.What) {
					case ServiceProxy.MSG_REGION_ENTERED:
						{
							UUID uuid = msg.Obj != null ? ((ParcelUuid)msg.Obj).Uuid : null;
							int major = msg.Arg1;
							int minor = msg.Arg2;

							// Get the region reference from a storage or create a new one
							int hash = uuid != null ? uuid.GetHashCode() ^ (major << 16) ^ minor : 0;
							BeaconRegion region = mRegionsByHash.Get(hash, new BeaconRegion(uuid, major, minor));
							mRegionsByHash.Put(hash, region);

							mListener.OnEnterRegion(region);
							break;
						}
					case ServiceProxy.MSG_REGION_EXITED:
						{
							UUID uuid = ((ParcelUuid)msg.Obj).Uuid;
							int major = msg.Arg1;
							int minor = msg.Arg2;

							// Get the region reference from a storage or create a new one
							int hash = uuid != null ? uuid.GetHashCode() ^ (major << 16) ^ minor : 0;
							BeaconRegion region = mRegionsByHash.Get(hash, new BeaconRegion(uuid, major, minor));
							mRegionsByHash.Put(hash, region);

							mListener.OnExitRegion(region);
							break;
						}
				}
			}
		}

		/**
	 * Constructs the new Beacon service connection that may be used to bind the activity with the running service.
	 */
		public BeaconServiceConnection() {
			mBeaconsMessengers = new Dictionary<BeaconsListener, Messenger>();
			mRegionMessengers = new Dictionary<RegionListener, Messenger>();
		}

	
		public void OnServiceConnected(ComponentName name, IBinder service) {
			mService = new Messenger(service);
			OnServiceConnected();
		}

		public void OnServiceDisconnected(ComponentName name) {
			mService = null;
			OnServiceDisconnected();
		}

		/**
	 * Called when a connection to the Service has been established.
	 */
		public abstract void OnServiceConnected();

		/**
	 * Called when a connection to the service is lost. This typically happens when the process hosting the service has crashed or been killed. This does not remove the
	 * {@link BeaconServiceConnection} itself -- this binding to the service will remain active, and you will receive a call to onServiceConnected the next time the service is running.
	 */
		public abstract void OnServiceDisconnected();

		/**
	 * Starts ranging for beacons in the region. The given listener will be notified about the beacons matching the region every second, event if the list of beacons will be empty.
	 * 
	 * @param uuid
	 *            the region's service UUID. The UUID may be <code>null</code> or {@link BeaconRegion#ANY_UUID} - in that case the region will match any beacon.
	 * @param listener
	 *            the listener
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StartRangingBeaconsInRegion(UUID uuid, BeaconsListener listener) {
			return StartRangingBeaconsInRegion(uuid, BeaconRegion.ANY, BeaconRegion.ANY, listener);
		}

		/**
	 * Starts ranging for beacons in the region. The given listener will be notified about the beacons matching the region every second, event if the list of beacons will be empty.
	 * 
	 * @param uuid
	 *            the region's service UUID. The UUID may NOT be <code>null</code> OR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the region's major number
	 * @param listener
	 *            the listener
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StartRangingBeaconsInRegion(UUID uuid, int major, BeaconsListener listener) {
			return StartRangingBeaconsInRegion(uuid, major, BeaconRegion.ANY, listener);
		}

		/**
	 * Starts ranging for beacons in the region. The given listener will be notified about the beacons matching the region every second, event if the list of beacons will be empty.
	 * 
	 * @param uuid
	 *            the region's service UUID. The UUID may NOT be <code>null</code> OR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the region's major number
	 * @param minor
	 *            the region's minor number
	 * @param listener
	 *            the listener
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StartRangingBeaconsInRegion(UUID uuid, int major, int minor, BeaconsListener listener) {
			if (mService == null)
				return false;

			// Validate parameters
			if (listener == null)
				throw new NullPointerException("BeaconsListener may not by null.");
			if (uuid == null && (major != BeaconRegion.ANY || minor != BeaconRegion.ANY))
				throw new UnsupportedOperationException("UUID may not be null if major or minor number is specified.");
			if (major == BeaconRegion.ANY && minor != BeaconRegion.ANY)
				throw new UnsupportedOperationException("Minor number may not be specified if major number is not.");

			try {

				Messenger messenger;

				if (mBeaconsMessengers.ContainsKey(listener))
					messenger = mBeaconsMessengers[listener];
				else
					messenger = new Messenger(new BeaconsListenerHandler(listener, mRegionsByHash, mBeaconsByAddressHash));

				Message msg = Message.Obtain();
				msg.What = ServiceProxy.MSG_START_RANGING_BEACONS_IN_REGION;
				msg.Obj = uuid != null ? new ParcelUuid(uuid) : null;
				msg.Arg1 = major;
				msg.Arg2 = minor;
				msg.ReplyTo = messenger;
				mService.Send(msg);

				if (mBeaconsMessengers.ContainsKey(listener))
					mBeaconsMessengers[listener] = messenger;
				else
					mBeaconsMessengers.Add(listener, messenger);
			}
			catch (RemoteException e) {
				//Log.E(TAG, "An exception occured while sending message", e);
				return false;
			}
			return true;
		}

		/**
	 * Unregisters the listener and stops ranging for beacons in all regions that it was registered for.
	 * 
	 * @param listener
	 *            the previously registered listener.
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StopRangingBeaconsInRegion(BeaconsListener listener) {
			if (mService == null)
				return false;

			Messenger messenger;

			if (mBeaconsMessengers.ContainsKey(listener)) {
				messenger = mBeaconsMessengers[listener];
				mBeaconsMessengers.Remove(listener);
			}
			else
				return true;

			try {
				Message msg = Message.Obtain();
				msg.What = ServiceProxy.MSG_STOP_RANGING_BEACONS_IN_REGION;
				msg.ReplyTo = messenger;
				mService.Send(msg);
			}
			catch (RemoteException e) {
				//Log.e(TAG, "An exception occured while sending message", e);
				return false;
			}
			return true;
		}

		/**
	 * Starts active monitoring for the region. The given listener will be notified about 'region entered' and 'region exited' events.
	 * 
	 * @param uuid
	 *            the region's service UUID. The UUID may be <code>null</code> or {@link BeaconRegion#ANY_UUID} - in that case the region will match any beacon.
	 * @param listener
	 *            the listener
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StartMonitoringForRegion(UUID uuid, RegionListener listener) {
			return StartMonitoringForRegion(uuid, BeaconRegion.ANY, BeaconRegion.ANY, listener);
		}

		/**
	 * Starts active monitoring for the region. The given listener will be notified about 'region entered' and 'region exited' events.
	 * 
	 * @param uuid
	 *            the region's service UUID. The UUID may NOT be <code>null</code> OR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the region's major number
	 * @param listener
	 *            the listener
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StartMonitoringForRegion(UUID uuid, int major, RegionListener listener) {
			return StartMonitoringForRegion(uuid, major, BeaconRegion.ANY, listener);
		}

		/**
	 * Starts active monitoring for the region. The given listener will be notified about 'region entered' and 'region exited' events.
	 * 
	 * @param uuid
	 *            the region's service UUID. The UUID may NOT be <code>null</code> OR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the region's major number
	 * @param minor
	 *            the region's minor number
	 * @param listener
	 *            the listener
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StartMonitoringForRegion(UUID uuid, int major, int minor, RegionListener listener) {
			if (mService == null)
				return false;

			// Validate parameters
			if (listener == null)
				throw new NullPointerException("RegionListener may not by null.");
			if (uuid == null && (major != BeaconRegion.ANY || minor != BeaconRegion.ANY))
				throw new UnsupportedOperationException("UUID may not be null if major or minor number is specified.");
			if (major == BeaconRegion.ANY && minor != BeaconRegion.ANY)
				throw new UnsupportedOperationException("Minor number may not be specified if major number is not.");

			try {
				Messenger messenger;

				if (mRegionMessengers.ContainsKey(listener))
					messenger = mRegionMessengers[listener];
				else
					messenger = new Messenger(new RegionListenerHandler(listener, mRegionsByHash));

				Message msg = Message.Obtain();
				msg.What = ServiceProxy.MSG_START_MONITORING_FOR_REGION;
				msg.Obj = uuid != null ? new ParcelUuid(uuid) : null;
				msg.Arg1 = major;
				msg.Arg2 = minor;
				msg.ReplyTo = messenger;
				mService.Send(msg);
				mRegionMessengers.Add(listener, messenger);
			}
			catch (RemoteException e) {
				//Log.e(TAG, "An exception occured while sending message", e);
				return false;
			}
			return true;
		}

		/**
	 * Unregisters the listener and stops monitoring for all regions that it was registered for.
	 * 
	 * @param listener
	 *            the previously registered listener.
	 * @return <code>true</code> if operation succeeded, <code>false</code> if service no longer exists.
	 */
		public bool StopMonitoringForRegion(RegionListener listener) {
			if (mService == null)
				return false;

			Messenger messenger;

			if (mRegionMessengers.ContainsKey(listener)) {
				messenger = mRegionMessengers[listener];
				mRegionMessengers.Remove(listener);
			}
			else
				return true;

			try {
				Message msg = Message.Obtain();
				msg.What = ServiceProxy.MSG_STOP_MONITORING_FOR_REGION;
				msg.ReplyTo = messenger;
				mService.Send(msg);
			}
			catch (RemoteException e) {
				//Log.e(TAG, "An exception occured while sending message", e);
				return false;
			}
			return true;
		}
	}

}