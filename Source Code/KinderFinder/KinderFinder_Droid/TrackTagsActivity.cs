﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "Track Tags", Icon = "@drawable/icon")]
	public class TrackTagsActivity : Activity {
		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;
		ImageView MapImageView;
		ProgressBar ProgressBar;
		TextView DownloadingText;

		/// <summary>
		/// The plain map, which has no dots overlayed onto it.
		/// </summary>
		Bitmap OriginalMap;

		/// <summary>
		/// The plain map with the tag positions overlayed onto it.
		/// </summary>
		Bitmap CurrentMap = null;

		Timer Timer;
		MediaPlayer AlarmPlayer = null;

		/// <summary>
		/// Keeps track of each tag that is out of range, so that we only display a notification once for each tag.
		/// </summary>
		List<string> OutOfRange = new List<string>();
		/// <summary>
		/// Remembers each tag's last location, so that when a tag is out of range, we can display its last know
		/// location.
		/// </summary>
		Dictionary<string, Point> PrevLocations = new Dictionary<string, Point>();

		public override bool OnCreateOptionsMenu(IMenu menu) {
			base.OnCreateOptionsMenu(menu);
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item) {
			base.OnOptionsItemSelected(item);

			switch (item.ItemId) {
				case Resource.Id.Menu_ChangeRestaurant:
					StartActivity(new Intent(this, typeof(RestaurantListActivity)));
					Finish();
					break;
				case Resource.Id.Menu_EditDetails:
					StartActivity(new Intent(this, typeof(EditDetailsActivity)));
					break;
				case Resource.Id.Menu_LogOut:
					Editor.Clear();
					Editor.Commit();
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

			Pref = GetSharedPreferences(Settings.Storage.PREFERENCES_FILE, 0);
			Editor = Pref.Edit();

			MapImageView = FindViewById<ImageView>(Resource.Id.Track_Map);
			ProgressBar = FindViewById<ProgressBar>(Resource.Id.Track_ProgressBar);
			DownloadingText = FindViewById<TextView>(Resource.Id.Track_DownloadingText);
			//AlarmPlayer = MediaPlayer.Create(this, Resource.Raw.OutOfRangeAlarm);

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
			MapImageView.Visibility = ViewStates.Gone;

			ThreadPool.QueueUserWorkItem(state => {
				string email = Pref.GetString(Settings.Keys.USERNAME, null);
				string restaurant = Pref.GetString(Settings.Keys.RESTAURANT_NAME, null);

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
					ProgressBar.Visibility = ViewStates.Gone;
					DownloadingText.Visibility = ViewStates.Gone;
					MapImageView.Visibility = ViewStates.Visible;

					/* No error; success! */
					if (errorMsg == null) {
						OriginalMap = AppTools.ResizeBitmap(bitmap, Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels);
						MapImageView.SetImageBitmap(OriginalMap);

						Timer = new Timer(UpdateMap);
						Timer.Change(0, Settings.Network.UPDATE_FREQUENCY);
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

		public struct Point {
			public double X;
			public double Y;

			public Point(double x, double y) {
				X = x;
				Y = y;
			}
		}

		static bool IsPointInRange(double x, double y) {
			return !x.Equals(Settings.SpecialPoints.OUT_OF_RANGE) || !y.Equals(Settings.SpecialPoints.OUT_OF_RANGE);
		}

		static bool IsPointValid(double x, double y) {
			return !x.Equals(Settings.SpecialPoints.TRANSMITTER_PROBLEM) || !y.Equals(Settings.SpecialPoints.TRANSMITTER_PROBLEM);
		}

		/// <summary>
		/// Updates the map by drawing the locations of the tracked tags.
		/// </summary>
		/// <param name="state">Not used.</param>
		void UpdateMap(object state) {
			string email = Pref.GetString(Settings.Keys.USERNAME, null);

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
					RunOnUiThread(() => Toast.MakeText(this, "Unable to retrieve data from server: " + response.StatusCode, ToastLength.Short).Show());
					break;
			}

			if (CurrentMap != null && CurrentMap != OriginalMap && !CurrentMap.IsRecycled)
				CurrentMap.Recycle();

			//var newBitmap = Bitmap.CreateBitmap(OriginalMap.Width, OriginalMap.Height, Bitmap.Config.Rgb565);
			CurrentMap = Bitmap.CreateBitmap(OriginalMap.Width, OriginalMap.Height, Bitmap.Config.Rgb565);
			var canvas = new Canvas(CurrentMap);
			var paint = new Paint();

			paint.SetStyle(Paint.Style.Fill);
			paint.TextSize = Settings.Map.OVERLAY_TEXT_SIZE;
			canvas.DrawBitmap(OriginalMap, 0, 0, null); // draw map normally.

			// If we could retrieve the locations, draw them:
			if (locations != null) {
				foreach (var data in locations) {
					int scaledX = 0;
					int scaledY = 0;
					bool outOfRange = false;

					// Tag out of range; play alarm and load last position:
					if (!IsPointInRange(data.PosX, data.PosY)) {
						// If we haven't displayed a warning for the tag yet:
						if (!OutOfRange.Contains(data.Name)) {
							OutOfRange.Add(data.Name);
							PlayAlarm();
						}

						// If we don't have a last known position for the tag, skip it:
						if (!PrevLocations.ContainsKey(data.Name))
							continue;

						// Otherwise, display its last known position:
						scaledX = (int)(PrevLocations[data.Name].X * OriginalMap.Width);
						scaledY = (int)(PrevLocations[data.Name].Y * OriginalMap.Height);
						outOfRange = true;
					}
					// The point will also be invalid if a transmitter isn't sending the tag's strength. Try to display
					// the tag's last known position.
					else if (!IsPointValid(data.PosX, data.PosY)) {
						// If we don't have a last known position for the tag, skip it:
						if (!PrevLocations.ContainsKey(data.Name))
							continue;

						// Otherwise, display its last known position:
						scaledX = (int)(PrevLocations[data.Name].X * OriginalMap.Width);
						scaledY = (int)(PrevLocations[data.Name].Y * OriginalMap.Height);
					}
					// Tag in range; position normally:
					else {
						scaledX = (int)(data.PosX * OriginalMap.Width);	
						scaledY = (int)(data.PosY * OriginalMap.Height);
						var point = new Point(data.PosX, data.PosY);	

						// Update tag's last position to its current one:
						if (!PrevLocations.ContainsKey(data.Name))
							PrevLocations.Add(data.Name, point);
						else
							PrevLocations[data.Name] = point;

						// Tag was previously out of range but isn't a	nymore:
						if (OutOfRange.Contains(data.Name)) {
							OutOfRange.Remove(data.Name);
							var dialog = new AlertDialog.Builder(this);
							dialog.SetTitle("Tag In Range");

							string info = "A tag is back in range. ";

							if (OutOfRange.Count > 0) {
								info += "These tags are still out of range:\n";

								foreach (var tag in OutOfRange) {
									info += "\nTag: " + tag + "\n";
									info += "Child: " + Pref.GetString(tag + Settings.Keys.TAG_NAME, Settings.Map.UNKNOWN_NAME_TEXT);
								}
							}
							else
								info += "All tags are in range!";

							dialog.SetMessage(info);
							dialog.SetNeutralButton("Ok", (sender, e) => CleanUpAlarm());
							RunOnUiThread(() => dialog.Show());
						}
					}

					string colour = Pref.GetString(data.Name + Settings.Keys.TAG_COLOUR, "");
					string name = Pref.GetString(data.Name + Settings.Keys.TAG_NAME, Settings.Map.UNKNOWN_NAME_TEXT);

					if (name.Equals(""))
						name = Settings.Map.UNKNOWN_NAME_TEXT;

					if (colour.Equals(""))
						colour = Settings.Map.DEFAULT_DOT_COLOUR;

					if (outOfRange) {
						name = "(!)" + name;
						colour = Settings.Map.PROBLEM_DOT_COLOUR;
					}

					int r = Convert.ToInt32(colour.Substring(0, 2), 16);
					int g = Convert.ToInt32(colour.Substring(2, 2), 16);
					int b = Convert.ToInt32(colour.Substring(4, 2), 16);

					paint.SetARGB(Settings.Map.DOT_COLOUR_ALPHA, r, g, b);
					canvas.DrawCircle(scaledX, scaledY, Settings.Map.DOT_SIZE_RADIUS, paint);
					canvas.DrawText(name, scaledX - 30, scaledY - 25, paint);
				}
			}

			// Display new map:
			RunOnUiThread(() => MapImageView.SetImageDrawable(new BitmapDrawable(Resources, CurrentMap)));
		}

		void CleanUpAlarm() {
			if (AlarmPlayer != null) {
				if (AlarmPlayer.IsPlaying)
					AlarmPlayer.Stop();

				AlarmPlayer.Release();
				AlarmPlayer = null;
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();

			CleanUpAlarm();
		}

		void PlayAlarm() {
			var dialog = new AlertDialog.Builder(this);
			dialog.SetTitle("Alert!");

			string info = "These tags are out of range:\n";

			foreach (var tag in OutOfRange) {
				info += "\nTag: " + tag + "\n";
				info += "Child: " + Pref.GetString(tag + Settings.Keys.TAG_NAME, Settings.Map.UNKNOWN_NAME_TEXT);
			}

			dialog.SetMessage(info);
			dialog.SetNeutralButton("Ok", (sender, e) => CleanUpAlarm());
			RunOnUiThread(() => dialog.Show());

			// Create media player if it doesn't exist:
			if (AlarmPlayer == null)
				AlarmPlayer = MediaPlayer.Create(this, Resource.Raw.OutOfRangeAlarm);

			// Start media player if it isn't playing yet:
			if (!AlarmPlayer.IsPlaying)
				AlarmPlayer.Start();
		}
	}
}

