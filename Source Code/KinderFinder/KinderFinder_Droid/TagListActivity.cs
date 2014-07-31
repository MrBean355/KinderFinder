using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "Test")]			
	public class TagListActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		ListView tagListView;
		Button linkButton;
		List<string> tags;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TagList);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			tagListView = FindViewById<ListView>(Resource.Id.TagList_List);
			linkButton = FindViewById<Button>(Resource.Id.TagList_Link);

			tagListView.ItemClick += ListItemClicked;
			linkButton.Click += LinkButtonClicked;

			LoadItems();
		}

		void LinkButtonClicked(object sender, EventArgs args) {
			var response = Utility.SendData("api/freetag", "");
			var list = Utility.ParseJSON(response.Body);

			Console.WriteLine("--> Free <--");

			foreach (var item in list)
				Console.WriteLine("--> " + item);
		}

		/// <summary>
		/// Executes when the user presses a tag in the list.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		void ListItemClicked(object sender, AdapterView.ItemClickEventArgs args) {
			var alert = new AlertDialog.Builder(this);
			var input = new EditText(this);
			string tag = tags[args.Position];

			input.InputType = Android.Text.InputTypes.TextFlagCapWords;

			alert.SetTitle("Assign Tag");
			alert.SetMessage("Child using the tag: " + tag);
			alert.SetView(input);

			/* When OK is clicked, save the child's name. */
			alert.SetPositiveButton("OK", (s, e) => {
				string name = input.Text;

				if (name.Length < 3)
					Toast.MakeText(this, "Name must be at least 3 characters", ToastLength.Long).Show();
				else {
					editor.PutString(tag, name);
					editor.Commit();
					LoadItems(); // reload list.
					Toast.MakeText(this, "Success", ToastLength.Short).Show();
				}
			});

			/* When Cancel is clicked, do nothing special. */
			alert.SetNegativeButton("Cancel", (s, e) => {
			});

			alert.Show();
		}

		/// <summary>
		/// Populates the tag list by contacting the server and getting a list of linked tags.
		/// </summary>
		void LoadItems() {
			ServerResponse reply = Utility.SendData("api/taglist", "{ \"EmailAddress\":\"" + pref.GetString(Globals.KEY_USERNAME, "") + "\" }");
			tags = Utility.ParseJSON(reply.Body);
			var items = new List<string>(tags);

			for (int i = 0; i < items.Count; i++) {
				string name = pref.GetString(tags[i], "");

				if (name.Equals(""))
					name = "(none assigned)";

				items[i] += " - " + name;
			}

			tagListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, items);
		}
	}
}

