using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Android.Graphics.Drawables;
using System;
using System.Collections.Generic;

namespace KinderFinder_Droid {

	[Activity(Label = "Track Tags")]			
	public class TrackTagsActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		ImageView mapImage;
		ProgressBar progressBar;
		TextView downloadingText;
		Bitmap OriginalMap;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TrackTags);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			mapImage = FindViewById<ImageView>(Resource.Id.Track_Map);
			progressBar = FindViewById<ProgressBar>(Resource.Id.Track_ProgressBar);
			downloadingText = FindViewById<TextView>(Resource.Id.Track_DownloadingText);

			LoadImage();
		}

		void LoadImage() {
			// Hide map image:
			mapImage.Visibility = Android.Views.ViewStates.Gone;

			ThreadPool.QueueUserWorkItem(state => {
				string data = "{" +
				              "\"EmailAddress\":\"" + pref.GetString(Globals.KEY_USERNAME, "") +
				              "\"}";

				string rest = pref.GetString(Globals.KEY_RESTAURANT_NAME, "");
				var response = Utility.SendData("api/mapsize", data);
				int serverSize = int.Parse(response.Body);
				bool success = false;
				Bitmap bitmap = null;

				var cache = new MapCache();

				if (cache.IsMapSame(rest, serverSize)) {
					bitmap = cache.GetStoredMap(rest);
					success = true;
				}
				else {
					response = Utility.SendData("api/map", data);

					switch (response.StatusCode) {
						case HttpStatusCode.OK:
							bitmap = BitmapFactory.DecodeByteArray(response.Bytes, 0, response.Bytes.Length);
							cache.AddMap(rest, serverSize, bitmap);
							success = true;
							break;
					}
				}

				RunOnUiThread(() => {
					progressBar.Visibility = Android.Views.ViewStates.Gone;
					downloadingText.Visibility = Android.Views.ViewStates.Gone;
					mapImage.Visibility = Android.Views.ViewStates.Visible;

					if (success) {
						OriginalMap = Utility.ResizeBitmap(bitmap, Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels);
						mapImage.SetImageBitmap(OriginalMap);

						var timer = new Timer(UpdateMap);
						timer.Change(0, 1000);
					}
					else {
						Toast.MakeText(this, "Unable to download map", ToastLength.Short).Show();
					}
				});
			});
		}

		public void UpdateMap(object state) {
			var builder = new JsonBuilder();
			builder.AddEntry("EmailAddress", pref.GetString(Globals.KEY_USERNAME, ""));

			var	response = Utility.SendData("api/track", builder.ToString());
			List<string> locations = null;

			switch (response.StatusCode) {
				case HttpStatusCode.OK:
					locations = Utility.ParseJSON(response.Body);
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
			paint.SetARGB(255, 0, 200, 0);
			canvas.DrawBitmap(OriginalMap, 0, 0, null); // draw map normally.

			// If we could retrieve the locations, draw them:
			if (locations != null && locations.Count % 2 == 0) {
				for (int i = 0; i < locations.Count; i += 2) {
					double loc1;
					double loc2;

					if (double.TryParse(locations[i], out loc1) && double.TryParse(locations[i + 1], out loc2)) {
						int x = (int)(loc1 * OriginalMap.Width);
						int y = (int)(loc2 * OriginalMap.Height);
						canvas.DrawCircle(x, y, 10, paint);
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

