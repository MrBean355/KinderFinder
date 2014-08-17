using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "Linked Tags")]			
	public class TagListActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		Button trackButton;
		TextView warning;
		ListView tagListView;
		List<string> tags;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TagList);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			trackButton = FindViewById<Button>(Resource.Id.TagList_Track);
			warning = FindViewById<TextView>(Resource.Id.TagList_Warning);
			tagListView = FindViewById<ListView>(Resource.Id.TagList_List);

			trackButton.Click += (sender, e) => StartActivity(new Intent(this, typeof(TrackTagsActivity)));
			tagListView.ItemClick += ListItemClicked;
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
			var input = new EditText(this);
			string tag = tags[args.Position];

			input.InputType = Android.Text.InputTypes.TextFlagCapWords;
			input.Text = pref.GetString(tag, ""); // load existing child.
			input.SetSelection(input.Text.Length); // move cursor to end.

			alert.SetTitle("Assign Tag");
			alert.SetMessage("Enter the name of the child to assign this tag to:");
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
					Toast.MakeText(this, "Success", ToastLength.Long).Show();
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

			if (tags.Count == 0)
				warning.Visibility = Android.Views.ViewStates.Visible;
			else {
				for (int i = 0; i < items.Count; i++) {
					string name = pref.GetString(tags[i], "");

					if (name.Equals(""))
						name = "(none assigned)";

					items[i] += " - " + name;
				}

				warning.Visibility = Android.Views.ViewStates.Gone;
			}

			tagListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, items);
		}
	}
}

