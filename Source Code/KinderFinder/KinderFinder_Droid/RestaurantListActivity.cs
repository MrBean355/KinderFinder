using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;

namespace KinderFinder_Droid {

	[Activity(Label = "Link Restaurant")]			
	public class RestaurantListActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		ListView restListView;
		List<string> restaurants;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.RestaurantList);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			restListView = FindViewById<ListView>(Resource.Id.RestList_List);

			restListView.ItemClick += ListItemClicked;
		}

		/**
		 * When the activity is resumed, reload the list of linked tags.
		 */
		protected override void OnResume() {
			base.OnResume();

			LoadItems();
		}

		/// <summary>
		/// Executes when the user presses a tag in the list.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		void ListItemClicked(object sender, AdapterView.ItemClickEventArgs args) {
			var alert = new AlertDialog.Builder(this);
			string rest = restaurants[args.Position];

			alert.SetTitle("Link Restaurant");
			alert.SetMessage("Are you sure you want to link to this restaurant (" + rest + ")?");

			alert.SetPositiveButton("OK", (s, e) => {
				string email = pref.GetString(Globals.KEY_USERNAME, "");
				string data = "{" +
				              "\"EmailAddress\":\"" + email + "\"," +
				              "\"Restaurant\":\"" + rest + "\"" +
				              "}";

				var reply = Utility.SendData("api/linkrestaurant", data);

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						Toast.MakeText(this, "Success!", ToastLength.Short).Show();
						StartActivity(new Intent(this, typeof(TagListActivity)));
						Finish();
						break;
					default:
						Toast.MakeText(this, "Server error. Please try again later", ToastLength.Short).Show();
						break;
				}
			});

			alert.SetNegativeButton("Cancel", (s, e) => {
			});

			alert.Show();
		}

		/// <summary>
		/// Populates the tag list by contacting the server and getting a list of linked tags.
		/// </summary>
		void LoadItems() {
			ServerResponse reply = Utility.SendData("api/restaurantlist", null);
			restaurants = Utility.ParseJSON(reply.Body);
			restListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, restaurants);
		}
	}
}

