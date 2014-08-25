﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KinderFinder {

	[Activity(Label = "Linked Tags", Icon = "@drawable/icon")]
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

			pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
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
				editor.PutString(tag, input.Text);
				editor.Commit();
				LoadItems(); // reload list.
				Toast.MakeText(this, "Success", ToastLength.Long).Show();
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
			string email = pref.GetString(Settings.Keys.USERNAME, null);

			if (email == null) {
				Toast.MakeText(this, Settings.Errors.LOCAL_DATA_ERROR, ToastLength.Long).Show();
				return;
			}

			var builder = new JsonBuilder();
			builder.AddEntry("EmailAddress", email);

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/taglist", builder.ToString());
				string message = null;

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						tags = AppTools.ParseJSON(reply.Body);
						break;
					case HttpStatusCode.BadRequest:
						message = "Invalid user details";
						break;
					default:
						message = Settings.Errors.SERVER_ERROR;
						break;
				}

				/* Error message was set; create empty list. */
				if (message != null)
					tags = new List<string>();

				/* Temporary list for displaying children's names next to tags. */
				var listItems = new List<string>(tags);

				RunOnUiThread(() => {
					/* No linked tags; show warning message. */
					if (tags.Count == 0)
						warning.Visibility = Android.Views.ViewStates.Visible;
					/* Has linked tags; display them. */
					else {
						for (int i = 0; i < tags.Count; i++) {
							string name = pref.GetString(tags[i], ""); // load child's name.

							if (name.Equals(""))
								name = "(no child assigned)";

							listItems[i] += ": " + name;
						}

						warning.Visibility = Android.Views.ViewStates.Gone; // hide warning message.
					}

					/* Show temporary list. */
					tagListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, listItems);
				});
			});
		}
	}
}

