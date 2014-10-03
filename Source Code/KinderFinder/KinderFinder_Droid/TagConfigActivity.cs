using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "Configure Tag")]			
	public class TagConfigActivity : Activity {
		static Dictionary<string, string> Colours;

		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		EditText NameBox;
		Spinner ColourSpinner;
		Button CancelButton, SaveButton;
		string CurrentTag;

		static TagConfigActivity() {
			Colours = new Dictionary<string, string>();
			Colours.Add("Black", "000000");
			Colours.Add("Red", "FF0000");
			Colours.Add("Green", "00FF00");
			Colours.Add("Blue", "0000FF");
			Colours.Add("Yellow", "FFD700");
			Colours.Add("Purple", "FF00FF");
			Colours.Add("Pink", "FFC0CB");
			Colours.Add("Orange", "FF8C00");
		}

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TagConfig);

			pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
			editor = pref.Edit();

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

			CurrentTag = pref.GetString("currenttag", "");

			if (!CurrentTag.Equals("")) {
				string name = pref.GetString(CurrentTag + Settings.Keys.TAG_NAME, "");
				string colour = pref.GetString(CurrentTag + Settings.Keys.TAG_COLOUR, "");

				NameBox.Text = name;

				if (!colour.Equals("")) {
					int pos = 0;

					foreach (var item in Colours) {
						if (colour.Equals(item.Value)) {
							ColourSpinner.SetSelection(pos);
							break;
						}

						pos++;
					}
				}
			}
			else {
				Toast.MakeText(this, "Unable to load tag; try again", ToastLength.Short).Show();
				Finish();
			}
		}

		void SaveButtonClicked(object sender, EventArgs args) {
			string colour = ColourSpinner.SelectedItem.ToString();
			editor.PutString(CurrentTag + Settings.Keys.TAG_NAME, NameBox.Text);
			editor.PutString(CurrentTag + Settings.Keys.TAG_COLOUR, Colours[colour]);
			editor.Commit();

			Finish();
		}

		void CancelButtonClicked(object sender, EventArgs args) {
			Finish();
		}
	}
}

