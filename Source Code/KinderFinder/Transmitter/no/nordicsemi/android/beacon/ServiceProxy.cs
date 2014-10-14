

/*import java.util.UUID;

import android.app.Notification;
import android.content.Context;
import android.content.Intent;
import android.os.ParcelUuid;*/
using Android.Content;
using Java.Util;
using Android.App;
using Java.Lang;
using Android.OS;

namespace no.nordicsemi.android.beacon {

	/**
 * This class provides the public API of the Beacon Service. You must install the "nRF Beacon Service" application prior to use it.
 */
	public class ServiceProxy {
		private static string SERVICE_PACKAGE_NAME = "no.nordicsemi.android.beacon.service";
		public static string REGION_DIR_MIME_TYPE = "vnd.android.cursor.dir/vnd.no.nordicsemi.beacon.region";
		/** The action used to bind to the service */
		public static string ACTION_BIND = "no.nordicsemi.android.beacon.action.BIND";

		/** The region UUID */
		public static string EXTRA_UUID = "no.nordicsemi.android.beacon.extra.UUID";
		/** The region major number */
		public static string EXTRA_MAJOR = "no.nordicsemi.android.beacon.extra.MAJOR";
		/** The region minor number */
		public static string EXTRA_MINOR = "no.nordicsemi.android.beacon.extra.MINOR";
		/** The application package name. This is used to distinguish the same regions registered by different applications. */
		public static string EXTRA_PACKAGE_NAME = "no.nordicsemi.android.beacon.extra.PACKAGE_NAME";
		/** The notification to show if region has been entered. */
		public static string EXTRA_NOTIFICATION = "no.nordicsemi.android.beacon.extra.NOTIFICATION";

		/** Number of beacons in the region. This extra field is returned to {@link BeaconServiceConnection} when the listener has been registered for ranging beacons in the region. */
		public static string EXTRA_COUNT = "no.nordicsemi.android.beacon.extra.COUNT";
		/** The addresses of beacons found in the region. This extra field is returned to {@link BeaconServiceConnection} when the listener has been registered for ranging beacons in the region. */
		public static string EXTRA_ADDRESSES = "no.nordicsemi.android.beacon.extra.ADDRESSES";
		/**
	 * The list of service UUID of beacons found in the region. This extra field is returned to {@link BeaconServiceConnection} when the listener has been registered for ranging beacons in the region.
	 */
		public static string EXTRA_UUIDS = "no.nordicsemi.android.beacon.extra.UUIDS";
		/**
	 * The majors & minors of beacons found in the region (0xMMMMmmmm). This extra field is returned to {@link BeaconServiceConnection} when the listener has been registered for ranging beacons in the
	 * region.
	 */
		public static string EXTRA_NUMBERS = "no.nordicsemi.android.beacon.extra.NUMBERS";
		/**
	 * The estimated distances to beacons that has been found in the region. This extra field is returned to {@link BeaconServiceConnection} when the listener has been registered for ranging beacons
	 * in the region.
	 */
		public static string EXTRA_ACCURACIES = "no.nordicsemi.android.beacon.extra.ACCURACIES";
		/**
	 * The average RSSI value of last 10 samples received from the beacon. This extra field is returned to {@link BeaconServiceConnection} when the listener has been registered for ranging beacons in
	 * the region.
	 */
		public static string EXTRA_RSSI_VALUES = "no.nordicsemi.android.beacon.extra.RSSI_VALUES";

		public const int MSG_STOP_MONITORING_FOR_REGION = 10;
		public const int MSG_START_MONITORING_FOR_REGION = 11;
		public const int MSG_REGION_ENTERED = 12;
		public const int MSG_REGION_EXITED = 13;
		public const int MSG_STOP_RANGING_BEACONS_IN_REGION = 20;
		public const int MSG_START_RANGING_BEACONS_IN_REGION = 21;
		public const int MSG_BEACONS_IN_REGION = 22;

		/**
	 * See {@link #startMonitoringForRegion(Context, UUID, int, int, Notification)} for documentation.
	 * 
	 * @param context
	 *            the calling activity context
	 * @param uuid
	 *            the iBeacon service UUID (128-bit). This may NOT be <code>null</code> NOR {@link BeaconRegion#ANY_UUID}.
	 * @param notification
	 *            the notification that will be shown if the device will enter specified region. Use {@link Notification.Builder#setLargeIcon(android.graphics.Bitmap)} to set the notification icon.
	 *            The small icon resource will be overwritten. The notification flag {@link Notification#FLAG_AUTO_CANCEL} will be added by the service.
	 * @return <code>false</code> if the "nRF Beacon Service" application is not installed or an error occur during starting the service. <code>True</code> otherwise.
	 */
		public static bool startMonitoringForRegion(Context context, UUID uuid, Notification notification) {
			return startMonitoringForRegion(context, uuid, BeaconRegion.ANY, BeaconRegion.ANY, notification);
		}

		/**
	 * See {@link #startMonitoringForRegion(Context, UUID, int, int, Notification)} for documentation.
	 * 
	 * @param context
	 *            the calling activity context
	 * @param uuid
	 *            the iBeacon service UUID (128-bit). This may NOT be <code>null</code> NOR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the major number. For any number use {@link BeaconRegion#ANY}
	 * @param notification
	 *            notification that is shown if the device enters a specified region. Use {@link Notification.Builder#setLargeIcon(android.graphics.Bitmap)} to set the notification icon.
	 *            The small icon resource will be overwritten. The notification flag {@link Notification#FLAG_AUTO_CANCEL} will be added by the service.
	 * @return <code>false</code> if the "nRF Beacon Service" application is not installed or an error occur during starting the service. <code>True</code> otherwise.
	 */
		public static bool startMonitoringForRegion(Context context, UUID uuid, int major, Notification notification) {
			return startMonitoringForRegion(context, uuid, major, BeaconRegion.ANY, notification);
		}

		/**
	 * <p>
	 * Starts monitoring for the region with specified parameters.
	 * </p>
	 * <p>
	 * This method requires the "nRF Beacon Service" application installed on the device. This method starts the service (if it has not been started by another application) which will periodically
	 * scan for beacons in the range. If the service could not be started (i.e. the app is not installed), the <code>false</code> is returned. If a first beacon matching the region's parameters is
	 * found, the Service will show the specified {@link Notification}. The notification will not be shown if there is already such a beacon in the device range during call. If the signal from the
	 * last beacon matching the region will be lost, the notification will be canceled. The second call with the same region parameters will overwrite the notification. Use
	 * {@link #stopMonitoringForRegion(Context, UUID, int, int)} to stop monitoring for region.
	 * </p>
	 * <p>
	 * The service scans for BLE devices for approximately one second every 15 seconds in order to save the battery and only if at least one region has been registered by any application.
	 * </p>
	 * 
	 * @param context
	 *            the calling activity context
	 * @param uuid
	 *            the iBeacon service UUID (128-bit). This may NOT be <code>null</code> NOR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the major number. For any number use {@link BeaconRegion#ANY}
	 * @param minor
	 *            the major number. For any number use {@link BeaconRegion#ANY}
	 * @param notification
	 *            notification that is shown if the device will enter specified region. Use {@link Notification.Builder#setLargeIcon(android.graphics.Bitmap)} to set the notification icon.
	 *            The small icon resource will be overwritten. The notification flag {@link Notification#FLAG_AUTO_CANCEL} will be added by the service.
	 * @return <code>false</code> if the "nRF Beacon Service" application is not installed or an error occur during starting the service. <code>True</code> otherwise.
	 */
		public static bool startMonitoringForRegion(Context context, UUID uuid, int major, int minor, Notification notification) {
			if (context == null)
				throw new NullPointerException("Context may not be null");
			if (uuid == null)
				throw new NullPointerException("UUID may not be null");

			Intent service = new Intent(Intent.ActionInsert);
			service.SetPackage(SERVICE_PACKAGE_NAME);
			service.SetType(REGION_DIR_MIME_TYPE);
			service.PutExtra(EXTRA_UUID, new ParcelUuid(uuid));
			service.PutExtra(EXTRA_MAJOR, major);
			service.PutExtra(EXTRA_MINOR, minor);
			//service.PutExtra(EXTRA_PACKAGE_NAME, context.GetPackageName());

			service.PutExtra(EXTRA_NOTIFICATION, notification);
			return context.StartService(service) != null;
		}

		/**
	 * See {@link #stopMonitoringForRegion(Context, UUID, int, int)} for documentation.
	 * 
	 * @param context
	 *            the calling activity context
	 * @param uuid
	 *            the beacon service UUID (128-bit). This may NOT be <code>null</code> NOR {@link BeaconRegion#ANY_UUID}.
	 * @return <code>false</code> if the "nRF Beacon Service" application is not installed or an error occur during starting the service. <code>True</code> otherwise.
	 */
		public static bool stopMonitoringForRegion(Context context, UUID uuid) {
			return stopMonitoringForRegion(context, uuid, BeaconRegion.ANY, BeaconRegion.ANY);
		}

		/**
	 * See {@link #stopMonitoringForRegion(Context, UUID, int, int)} for documentation.
	 * 
	 * @param context
	 *            the calling activity context
	 * @param uuid
	 *            the beacon service UUID (128-bit). This may NOT be <code>null</code> NOR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the major number. For any number use {@link BeaconRegion#ANY}
	 * @return <code>false</code> if the "nRF Beacon Service" application is not installed or an error occur during starting the service. <code>True</code> otherwise.
	 */
		public static bool stopMonitoringForRegion(Context context, UUID uuid, int major) {
			return stopMonitoringForRegion(context, uuid, major, BeaconRegion.ANY);
		}

		/**
	 * Stops monitoring for region with given properties. If the region was the only one monitored by the Service, the Service will stop itself.
	 * 
	 * @param context
	 *            the calling activity context
	 * @param uuid
	 *            the iBeacon service UUID (128-bit). This may NOT be <code>null</code> NOR {@link BeaconRegion#ANY_UUID}.
	 * @param major
	 *            the major number. For any number use {@link BeaconRegion#ANY}
	 * @param minor
	 *            the major number. For any number use {@link BeaconRegion#ANY}
	 * @return <code>false</code> if the "nRF Beacon Service" application is not installed or an error occur during starting the service. <code>True</code> otherwise.
	 */
		public static bool stopMonitoringForRegion(Context context, UUID uuid, int major, int minor) {
			if (context == null)
				throw new NullPointerException("Context may not be null");
			if (uuid == null)
				throw new NullPointerException("UUID may not be null");

			Intent service = new Intent(Intent.ActionDelete);
			service.SetPackage(SERVICE_PACKAGE_NAME);
			service.SetType(REGION_DIR_MIME_TYPE);
			service.PutExtra(EXTRA_UUID, new ParcelUuid(uuid));
			service.PutExtra(EXTRA_MAJOR, major);
			service.PutExtra(EXTRA_MINOR, minor);
			//service.PutExtra(EXTRA_PACKAGE_NAME, context.GetPackageName());
			return context.StartService(service) != null;
		}

		/**
	 * <p>
	 * Binds to the Beacon Service. See {@link BeaconServiceConnection} for the service API.
	 * </p>
	 * <p>
	 * This method requires the "nRF Beacon Service" application installed on the device. Use it to register the monitoring or ranging listeners that notify about beacons directly into the
	 * application. The notifications will be sent every second.
	 * </p>
	 * 
	 * @param context
	 *            the calling activity context
	 * @param connection
	 *            the service connection used to start or stop active monitoring for region or ranging for beacons
	 * @return <code>false</code> if the "nRF Beacon Service" application is not installed or an error occur during starting the service. <code>True</code> otherwise.
	 */
		public static bool bindService(Context context, BeaconServiceConnection connection) {
			if (context == null)
				throw new NullPointerException("Context may not be null");

			Intent service = new Intent(ACTION_BIND);
			service.SetPackage(SERVICE_PACKAGE_NAME);
			return context.BindService(service, connection, Bind.AutoCreate);
		}

		/**
	 * Unbinds the service. This will stop the service if no more Activities are binded and no region are being monitored in passive scanning.
	 * 
	 * @param context
	 *            the calling activity context
	 * @param connection
	 *            the previously registered connection
	 */
		public static void unbindService(Context context, BeaconServiceConnection connection) {
			if (context == null)
				throw new NullPointerException("Context may not be null");

			context.UnbindService(connection);
		}
	}
}