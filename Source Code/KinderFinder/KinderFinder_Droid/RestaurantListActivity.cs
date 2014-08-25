using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KinderFinder {

	[Activity(Label = "Link Restaurant", Icon = "@drawable/icon")]
	public class RestaurantListActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		ListView restListView;
		List<string> restaurants;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.RestaurantList);

			pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			restListView = FindViewById<ListView>(Resource.Id.RestList_List);

			restListView.ItemClick += ListItemClicked;
		}

		/// <summary>
		/// Reload restaurant list when activity is resumed.
		/// </summary>
		protected override void OnResume() {
			base.OnResume();

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/restaurantlist", null);
				string errorMsg = null;

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						restaurants = AppTools.ParseJSON(reply.Body);
						RunOnUiThread(() => restListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, restaurants));
						break;
					default:
						errorMsg = Settings.Errors.SERVER_ERROR;
						break;
				}

				if (errorMsg != null)
					RunOnUiThread(() => Toast.MakeText(this, errorMsg, ToastLength.Short).Show());
			});
		}

		/// <summary>
		/// Executes when the user presses a tag in the list.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		void ListItemClicked(object sender, AdapterView.ItemClickEventArgs args) {
			string email = pref.GetString(Settings.Keys.USERNAME, null);

			if (email == null) {
				Toast.MakeText(this, Settings.Errors.LOCAL_DATA_ERROR, ToastLength.Long).Show();
				return;
			}

			string restaurant = restaurants[args.Position];
			var alert = new AlertDialog.Builder(this);

			alert.SetTitle("Link Restaurant");
			alert.SetMessage("Are you sure you want to link to " + restaurant + "?");

			alert.SetPositiveButton("OK", (s, e) => {
				var builder = new JsonBuilder();
				builder.AddEntry("EmailAddress", email);
				builder.AddEntry("Restaurant", restaurant);

				var reply = AppTools.SendRequest("api/linkrestaurant", builder.ToString());

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						Toast.MakeText(this, "Successfully linked!", ToastLength.Short).Show();
						editor.PutString(Settings.Keys.RESTAURANT_NAME, restaurant);
						editor.Commit();
						StartActivity(new Intent(this, typeof(TagListActivity)));
						Finish();
						break;
					default:
						Toast.MakeText(this, Settings.Errors.SERVER_ERROR, ToastLength.Short).Show();
						break;
				}
			});

			alert.SetNegativeButton("Cancel", (s, e) => {
			});

			alert.Show();
		}
	}
}

