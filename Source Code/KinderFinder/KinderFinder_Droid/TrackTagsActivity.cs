﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;

using KinderFinder.Utility;
using Android.Media;

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
		MediaPlayer AlarmPlayer;

		public override bool OnCreateOptionsMenu(Android.Views.IMenu menu) {
			base.OnCreateOptionsMenu(menu);
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(Android.Views.IMenuItem item) {
			base.OnOptionsItemSelected(item);

			switch (item.ItemId) {
				case Resource.Id.Menu_LogOut:
					editor.Clear();
					editor.Commit();
					StartActivity(new Intent(this, typeof(MainActivity)));
					Finish();
					break;
				case Resource.Id.Menu_Exit:
					System.Environment.Exit(0);
					break;
				default:
					Toast.MakeText(this, "Unknown menu item selected", ToastLength.Short).Show();
					break;
			}

			return base.OnOptionsItemSelected(item);
		}

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
		/// When the screen is closed, stop the timer.
		/// </summary>
		protected override void OnStop() {
			base.OnStop();

			if (Timer != null)
				Timer.Dispose();
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

		public struct TagData {
			public string Name;
			public double PosX;
			public double PosY;
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
			List<TagData> locations = null;

			switch (response.StatusCode) {
				case HttpStatusCode.OK:
					locations = Deserialiser<List<TagData>>.Run(response.Body);
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
			paint.TextSize = Settings.Map.OVERLAY_TEXT_SIZE;
			canvas.DrawBitmap(OriginalMap, 0, 0, null); // draw map normally.

			// If we could retrieve the locations, draw them:
			if (locations != null) {
				foreach (var data in locations) {
					if (data.PosX.Equals(-100.0) && data.PosY.Equals(-100.0)) {
						PlayAlarm(data.Name);
						continue;
					}

					int x = (int)(data.PosX * OriginalMap.Width);
					int y = (int)(data.PosY * OriginalMap.Height);
					string colour = pref.GetString(data.Name + Settings.Keys.TAG_COLOUR, "");
					string name = pref.GetString(data.Name + Settings.Keys.TAG_NAME, Settings.Map.UNKNOWN_NAME_TEXT);

					if (name.Equals("")) {
						name = Settings.Map.UNKNOWN_NAME_TEXT;
					}

					if (!colour.Equals("")) {
						int r = Convert.ToInt32(colour.Substring(0, 2), 16);
						int g = Convert.ToInt32(colour.Substring(2, 2), 16);
						int b = Convert.ToInt32(colour.Substring(4, 2), 16);
						paint.SetARGB(Settings.Map.DOT_COLOUR_ALPHA, r, g, b);
					}
					else {
						paint.SetARGB(Settings.Map.DOT_COLOUR_ALPHA, Settings.Map.DOT_COLOUR_RED, Settings.Map.DOT_COLOUR_GREEN, Settings.Map.DOT_COLOUR_BLUE);
					}

					canvas.DrawCircle(x, y, Settings.Map.DOT_SIZE_RADIUS, paint);
					canvas.DrawText(name, x - 30, y - 25, paint);
				}
			}

			// Display new map:
			RunOnUiThread(() => mapImage.SetImageDrawable(new BitmapDrawable(Resources, newBitmap)));
		}

		void PlayAlarm(string tagName) {
			var dialog = new AlertDialog.Builder(this);
			dialog.SetTitle("Alert!");
			dialog.SetMessage("A tag is out of range!\nTag Name: " + tagName);
			dialog.SetNeutralButton("Ok", (sender, e) => {
				AlarmPlayer.Stop();
			});
			dialog.Show();

			if (AlarmPlayer != null && AlarmPlayer.IsPlaying)
				return;

			AlarmPlayer = MediaPlayer.Create(this, Resource.Raw.OutOfRangeAlarm);
			AlarmPlayer.Start();
		}
	}
}

