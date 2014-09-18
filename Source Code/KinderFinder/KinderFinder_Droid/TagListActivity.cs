using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Views;

namespace KinderFinder {

	[Activity(Label = "Linked Tags", Icon = "@drawable/icon")]
	public class TagListActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		Button trackButton;
		TextView warning;
		ListView tagListView;
		List<string> tags;

		public override bool OnCreateOptionsMenu(Android.Views.IMenu menu) {
			base.OnCreateOptionsMenu(menu);
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(Android.Views.IMenuItem item) {
			base.OnOptionsItemSelected(item);

			switch (item.ItemId) {
				case Resource.Id.Menu_LogOut:
					editor.Clear();
					editor.Commit();
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
			StartActivity(new Intent(this, typeof(TagConfigActivity)));

			/*var alert = new AlertDialog.Builder(this);
			var tag = tags[args.Position];


			//nameBox.InputType = Android.Text.InputTypes.TextFlagCapWords;
			//nameBox.Text = pref.GetString(tag, ""); // load existing child.
			//nameBox.SetSelection(nameBox.Text.Length); // move cursor to end.

			LayoutInflater factory = LayoutInflater.From(this);
			var view = factory.Inflate(Resource.Layout.TagConfig, null);

			var testList = new List<String>();

			for (int i = 0; i < 10; i++) {
				testList.Add("Item " + (i + 1));
			}

			alert.SetTitle("Assign Tag");
			alert.SetMessage("Enter the name of the child to assign this tag to:");
			alert.SetView(view);


			/* When OK is clicked, save the child's name. *
			alert.SetPositiveButton("OK", (s, e) => {
				//editor.PutString(tag, nameBox.Text);
				//editor.Commit();
				LoadItems(); // reload list.
				Toast.MakeText(this, "Success", ToastLength.Long).Show();
			});

			/* When Cancel is clicked, do nothing special. *
			alert.SetNegativeButton("Cancel", (s, e) => {
			});

			alert.Show();*/
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

