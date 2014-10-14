using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Views;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "Linked Tags", Icon = "@drawable/icon")]
	public class TagListActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		Button refreshButton, trackButton;
		TextView warning;
		ListView tagListView;
		List<string> tags;

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

			refreshButton = FindViewById<Button>(Resource.Id.TagList_Refresh);
			trackButton = FindViewById<Button>(Resource.Id.TagList_Track);
			warning = FindViewById<TextView>(Resource.Id.TagList_Warning);
			tagListView = FindViewById<ListView>(Resource.Id.TagList_List);

			refreshButton.Click += (sender, e) => LoadItems();
			trackButton.Click += (sender, e) => StartActivity(new Intent(this, typeof(TrackTagsActivity)));
			tagListView.ItemClick += (sender, e) => {
				editor.PutString(Settings.Keys.CURRENT_TAG, tags[e.Position]);
				editor.Commit();
				StartActivity(new Intent(this, typeof(TagConfigActivity)));
			};
		}

		/**
		 * When the activity is resumed, reload the list of linked tags.
		 */
		protected override void OnResume() {
			base.OnResume();

			LoadItems();
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
						tags = Deserialiser<List<string>>.Run(reply.Body);
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
					if (tags.Count == 0) {
						warning.Visibility = ViewStates.Visible;
						trackButton.Enabled = false;
					}
					/* Has linked tags; display them. */
					else {
						for (int i = 0; i < tags.Count; i++) {
							string name = pref.GetString(tags[i] + Settings.Keys.TAG_NAME, ""); // load child's name.

							if (name.Equals(""))
								name = "(no child assigned)";

							listItems[i] += ": " + name;
						}

						warning.Visibility = ViewStates.Gone; // hide warning message.
						trackButton.Enabled = true;
					}

					/* Show temporary list. */
					tagListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, listItems);
				});
			});
		}
	}
}

