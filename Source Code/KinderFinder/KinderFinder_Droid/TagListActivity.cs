using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "Linked Tags", Icon = "@drawable/icon")]
	public class TagListActivity : ListActivity {
		List<string> Items;
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();
			loadItems();
		}

		/**
		 * Fetches a list of tags linked to the user and displays them in a list.
		 */
		private void loadItems() {
			ServerResponse reply = Utility.SendData("api/taglist", "{ \"EmailAddress\":\"" + pref.GetString(Globals.KEY_USERNAME, "") + "\" }");
			Items = Utility.ParseJSON(reply.Body);

			for (int i = 0; i < Items.Count; i++) {
				string name = pref.GetString(Items[i], "");

				if (name.Equals(""))
					name = "(none assigned)";

				Items[i] += " - " + name;
			}

			ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Items);
		}

		protected override void OnListItemClick(ListView list, View view, int position, long id) {
			AlertDialog.Builder alert = new AlertDialog.Builder(this);

			alert.SetTitle("Assign Child")
				.SetMessage("Enter the child to assign the tag to:");

			EditText input = new EditText(this);
			alert.SetView(input);

			alert.SetPositiveButton("OK", (sender, e) => {
				string name = input.Text;

				if (name.Length < 3)
					Toast.MakeText(this, "Name must be at least 3 characters", ToastLength.Long).Show();
				else {
					// TODO: Save child assigned to tag.
					Toast.MakeText(this, "Success", ToastLength.Short).Show();
				}
			});

			alert.SetNegativeButton("Cancel", (sender, e) => {
			});

			alert.Show();
		}
	}
}

