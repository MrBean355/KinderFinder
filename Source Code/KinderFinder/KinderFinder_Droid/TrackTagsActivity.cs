using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;

namespace KinderFinder {

	[Activity(Label = "Track Tags", Icon = "@drawable/icon")]
	public class TrackTagsActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		ImageView mapImage;
		ProgressBar progressBar;
		TextView downloadingText;
		Bitmap OriginalMap;
		Timer Timer;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TrackTags);

			pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			mapImage = FindViewById<ImageView>(Resource.Id.Track_Map);
			progressBar = FindViewById<ProgressBar>(Resource.Id.Track_ProgressBar);
			downloadingText = FindViewById<TextView>(Resource.Id.Track_DownloadingText);

			LoadMapImage();
		}

		/// <summary>
		/// Displays the map image by setting the ImageView's bitmap. Checks if the image is cached and loads it if
		/// there is no newer version on the server.
		/// </summary>
		void LoadMapImage() {
			// Hide map image:
			mapImage.Visibility = Android.Views.ViewStates.Gone;

			ThreadPool.QueueUserWorkItem(state => {
				string email = pref.GetString(Settings.Keys.USERNAME, null);
				string restaurant = pref.GetString(Settings.Keys.RESTAURANT_NAME, null);

				if (email == null || restaurant == null) {
					Toast.MakeText(this, Settings.Errors.LOCAL_DATA_ERROR, ToastLength.Long).Show();
					return;
				}

				var builder = new JsonBuilder();
				builder.AddEntry("EmailAddress", email);

				/* First, find out how big the server's version of the map is. */
				var response = AppTools.SendRequest("api/mapsize", builder.ToString());
				int serverSize;
				string errorMsg = null;
				Bitmap bitmap = null;
				var cache = new MapCache();

				int.TryParse(response.Body, out serverSize);

				/* Next, check if the local version of the map is the same size. If it is, simply load the cached map. */
				if (cache.IsMapSame(restaurant, serverSize))
					bitmap = cache.GetStoredMap(restaurant); // TODO: Check if map actually loaded.
				/* Maps are different; request map from server. */
				else {
					response = AppTools.SendRequest("api/map", builder.ToString());

					switch (response.StatusCode) {
						case HttpStatusCode.OK:
							bitmap = BitmapFactory.DecodeByteArray(response.Bytes, 0, response.Bytes.Length);
							cache.AddMap(restaurant, serverSize, bitmap);
							break;
						case HttpStatusCode.BadRequest:
							errorMsg = "Unable to load the restaurant's map";
							break;
						default:
							errorMsg = Settings.Errors.SERVER_ERROR;
							break;
					}
				}

				RunOnUiThread(() => {
					progressBar.Visibility = Android.Views.ViewStates.Gone;
					downloadingText.Visibility = Android.Views.ViewStates.Gone;
					mapImage.Visibility = Android.Views.ViewStates.Visible;

					/* No error; success! */
					if (errorMsg == null) {
						OriginalMap = AppTools.ResizeBitmap(bitmap, Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels);
						mapImage.SetImageBitmap(OriginalMap);

						Timer = new Timer(UpdateMap);
						Timer.Change(0, Settings.Map.UPDATE_FREQUENCY);
					}
					/* Something went wrong. */
					else
						Toast.MakeText(this, errorMsg, ToastLength.Short).Show();
				});
			});
		}

		/// <summary>
		/// Updates the map by drawing the locations of the tracked tags.
		/// </summary>
		/// <param name="state">Not used.</param>
		void UpdateMap(object state) {
			string email = pref.GetString(Settings.Keys.USERNAME, null);

			if (email == null) {
				Toast.MakeText(this, Settings.Errors.LOCAL_DATA_ERROR, ToastLength.Long).Show();
				Timer.Dispose(); // stop updating map.
				return;
			}

			var builder = new JsonBuilder();
			builder.AddEntry("EmailAddress", email);

			var	response = AppTools.SendRequest("api/track", builder.ToString());
			List<string> locations = null;

			switch (response.StatusCode) {
				case HttpStatusCode.OK:
					locations = AppTools.ParseJSON(response.Body);
					break;
				default:
					//Toast.MakeText(this, "Unable to retrieve data from server", ToastLength.Short).Show();
					Console.WriteLine("Something bad happened: " + response.StatusCode.ToString());
					break;
			}

			var newBitmap = Bitmap.CreateBitmap(OriginalMap.Width, OriginalMap.Height, Bitmap.Config.Rgb565);
			var canvas = new Canvas(newBitmap);
			var paint = new Paint();

			paint.SetStyle(Paint.Style.Fill);
			paint.SetARGB(Settings.Map.DOT_COLOUR_ALPHA, Settings.Map.DOT_COLOUR_RED, Settings.Map.DOT_COLOUR_GREEN, Settings.Map.DOT_COLOUR_BLUE);
			canvas.DrawBitmap(OriginalMap, 0, 0, null); // draw map normally.

			// If we could retrieve the locations, draw them:
			if (locations != null && locations.Count % 2 == 0) {
				for (int i = 0; i < locations.Count; i += 2) {
					double loc1;
					double loc2;

					if (double.TryParse(locations[i], out loc1) && double.TryParse(locations[i + 1], out loc2)) {
						int x = (int)(loc1 * OriginalMap.Width);
						int y = (int)(loc2 * OriginalMap.Height);
						canvas.DrawCircle(x, y, Settings.Map.DOT_SIZE_RADIUS, paint);
					}
					else
						RunOnUiThread(() => Toast.MakeText(this, "Server replied with some invalid data", ToastLength.Short).Show());
				}
			}

			// Display new map:
			RunOnUiThread(() => mapImage.SetImageDrawable(new BitmapDrawable(Resources, newBitmap)));
		}
	}
}

