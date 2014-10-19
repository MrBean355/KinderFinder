using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "Link Restaurant", Icon = "@drawable/icon")]
	public class RestaurantListActivity : Activity {
		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;

		ListView RestaurantList;
		EditText SearchBox;
		Button ClearButton;

		List<string> AllRestaurants;
		List<string> MatchingRestaurants;

		public override bool OnCreateOptionsMenu(IMenu menu) {
			base.OnCreateOptionsMenu(menu);
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);

			int items = menu.Size();
			menu.GetItem(items - 4).SetEnabled(false); // disable change restaurant item.

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
			SetContentView(Resource.Layout.RestaurantList);

			Pref = GetSharedPreferences(Settings.Storage.PREFERENCES_FILE, 0);
			Editor = Pref.Edit();

			RestaurantList = FindViewById<ListView>(Resource.Id.RestList_List);
			SearchBox = FindViewById<EditText>(Resource.Id.RestList_Search);
			ClearButton = FindViewById<Button>(Resource.Id.RestList_Clear);

			RestaurantList.ItemClick += ListItemClicked;
			SearchBox.TextChanged += SearchTextChanged;
			ClearButton.Click += (sender, e) => SearchBox.Text = "";
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
						AllRestaurants = Deserialiser<List<string>>.Run(reply.Body);
						MatchingRestaurants = AllRestaurants;
						RunOnUiThread(() => RestaurantList.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, AllRestaurants));
						break;
					default:
						errorMsg = Settings.Errors.SERVER_ERROR;
						break;
				}

				if (errorMsg != null)
					RunOnUiThread(() => Toast.MakeText(this, errorMsg, ToastLength.Short).Show());
			});
		}

		void SearchTextChanged(object sender, Android.Text.TextChangedEventArgs args) {
			string search = SearchBox.Text.ToLower();

			if (SearchBox.Text.Equals("")) {
				MatchingRestaurants = AllRestaurants;
			}
			else {
				MatchingRestaurants = new List<string>();

				foreach (string restaurant in AllRestaurants) {
					string str = restaurant.ToLower();

					if (str.Contains(search)) {
						MatchingRestaurants.Add(restaurant);
					}
				}
			}

			if (MatchingRestaurants != null) {
				RestaurantList.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, MatchingRestaurants);
			}
		}

		/// <summary>
		/// Executes when the user presses a tag in the list.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		void ListItemClicked(object sender, AdapterView.ItemClickEventArgs args) {
			string email = Pref.GetString(Settings.Keys.USERNAME, null);

			if (email == null) {
				Toast.MakeText(this, Settings.Errors.LOCAL_DATA_ERROR, ToastLength.Long).Show();
				return;
			}

			string restaurant = MatchingRestaurants[args.Position];
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
						Editor.PutString(Settings.Keys.RESTAURANT_NAME, restaurant);
						Editor.Commit();
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

