using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "Configure Tag", Icon = "@drawable/icon")]			
	public class TagConfigActivity : Activity {
		static Dictionary<string, string> Colours;

		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;
		EditText NameBox;
		Spinner ColourSpinner;
		Button CancelButton, SaveButton;
		string CurrentTag;

		static TagConfigActivity() {
			Colours = new Dictionary<string, string>();
			Colours.Add("Black", "000000");
			Colours.Add("Green", "00FF00");
			Colours.Add("Blue", "0000FF");
			Colours.Add("Yellow", "FFD700");
			Colours.Add("Purple", "FF00FF");
			Colours.Add("Pink", "FFC0CB");
			Colours.Add("Orange", "FF8C00");
		}

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
			SetContentView(Resource.Layout.TagConfig);

			Pref = GetSharedPreferences(Settings.Storage.PREFERENCES_FILE, 0);
			Editor = Pref.Edit();

			NameBox = FindViewById<EditText>(Resource.Id.TagConfig_Name);
			ColourSpinner = FindViewById<Spinner>(Resource.Id.TagConfig_Colour);
			CancelButton = FindViewById<Button>(Resource.Id.TagConfig_Cancel);
			SaveButton = FindViewById<Button>(Resource.Id.TagConfig_Save);

			CancelButton.Click += CancelButtonClicked;
			SaveButton.Click += SaveButtonClicked;

			var items = new List<string>();

			foreach (var item in Colours) {
				items.Add(item.Key);
			}

			ColourSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, items);
			CurrentTag = Pref.GetString(Settings.Keys.CURRENT_TAG, "");

			if (!CurrentTag.Equals("")) {
				string name = Pref.GetString(CurrentTag + Settings.Keys.TAG_NAME, "");
				string colour = Pref.GetString(CurrentTag + Settings.Keys.TAG_COLOUR, "");
				string toMatch = !colour.Equals("") ? colour : Settings.Map.DEFAULT_DOT_COLOUR;
				int pos = 0;

				NameBox.Text = name;

				foreach (var item in Colours) {
					if (item.Value.Equals(toMatch)) {
						ColourSpinner.SetSelection(pos);
						break;
					}

					pos++;
				}
			}
			else {
				Toast.MakeText(this, "Unable to load tag; try again", ToastLength.Short).Show();
				Finish();
			}
		}

		void SaveButtonClicked(object sender, EventArgs args) {
			string colour = ColourSpinner.SelectedItem.ToString();
			Editor.PutString(CurrentTag + Settings.Keys.TAG_NAME, NameBox.Text);
			Editor.PutString(CurrentTag + Settings.Keys.TAG_COLOUR, Colours[colour]);
			Editor.Commit();

			Finish();
		}

		void CancelButtonClicked(object sender, EventArgs args) {
			Finish();
		}
	}
}

