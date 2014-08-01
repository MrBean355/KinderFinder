
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "Link New Tag")]			
	public class LinkTagActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		ListView tagListView;
		List<string> tags;
		TextView warning;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.LinkTag);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			warning = FindViewById<TextView>(Resource.Id.LinkTag_Warning);
			tagListView = FindViewById<ListView>(Resource.Id.LinkTag_List);

			tagListView.ItemClick += ListItemClicked;

			LoadItems();
		}

		void ListItemClicked(object sender, AdapterView.ItemClickEventArgs args) {
			var alert = new AlertDialog.Builder(this);
			string tag = tags[args.Position];

			alert.SetTitle("Link New Tag");
			alert.SetMessage("Are you sure you want to link to the tag '" + tag + "'?");

			/* When Yes is clicked, attempt to link by contacting server. */
			alert.SetPositiveButton("Yes", (s, e) => {
				string data = "{" +
				              "\"EmailAddress\":\"" + pref.GetString(Globals.KEY_USERNAME, "") + "\"," +
				              "\"TagLabel\":\"" + tag + "\"" +
				              "}";

				var response = Utility.SendData("api/linktag", data);

				if (response.StatusCode == System.Net.HttpStatusCode.OK) {
					Toast.MakeText(this, "Successfully linked!", ToastLength.Long).Show();
					Finish();
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
					Toast.MakeText(this, "Bad request. Please try again", ToastLength.Long).Show();
				else
					Toast.MakeText(this, "Server error. Please try again later", ToastLength.Long).Show();
			});

			/* When Cancel is clicked, do nothing special. */
			alert.SetNegativeButton("Cancel", (s, e) => {
			});

			alert.Show();
		}

		void LoadItems() {
			ServerResponse reply = Utility.SendData("api/freetaglist", null);
			tags = Utility.ParseJSON(reply.Body);

			warning.Visibility = tags.Count == 0 ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
			tagListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, tags);
		}
	}
}

