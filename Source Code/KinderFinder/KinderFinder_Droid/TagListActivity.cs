using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "Linked Tags", Icon = "@drawable/icon")]
	public class TagListActivity : Activity {
		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;
		Button RefreshButton, TrackButton;
		TextView Warning;
		ListView TagListView;
		List<string> TagNames;

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
			SetContentView(Resource.Layout.TagList);

			Pref = GetSharedPreferences(Settings.Storage.PREFERENCES_FILE, 0);
			Editor = Pref.Edit();

			RefreshButton = FindViewById<Button>(Resource.Id.TagList_Refresh);
			TrackButton = FindViewById<Button>(Resource.Id.TagList_Track);
			Warning = FindViewById<TextView>(Resource.Id.TagList_Warning);
			TagListView = FindViewById<ListView>(Resource.Id.TagList_List);

			RefreshButton.Click += (sender, e) => LoadItems();
			TrackButton.Click += (sender, e) => StartActivity(new Intent(this, typeof(TrackTagsActivity)));
			TagListView.ItemClick += (sender, e) => {
				Editor.PutString(Settings.Keys.CURRENT_TAG, TagNames[e.Position]);
				Editor.Commit();
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
			string email = Pref.GetString(Settings.Keys.USERNAME, null);

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
						TagNames = Deserialiser<List<string>>.Run(reply.Body);
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
					TagNames = new List<string>();

				/* Temporary list for displaying children's names next to tags. */
				var listItems = new List<string>(TagNames);

				RunOnUiThread(() => {
					/* No linked tags; show warning message. */
					if (TagNames.Count == 0) {
						Warning.Visibility = ViewStates.Visible;
						TrackButton.Enabled = false;
					}
					/* Has linked tags; display them. */
					else {
						for (int i = 0; i < TagNames.Count; i++) {
							string name = Pref.GetString(TagNames[i] + Settings.Keys.TAG_NAME, ""); // load child's name.

							if (name.Equals(""))
								name = "(no child assigned)";

							listItems[i] += ": " + name;
						}

						Warning.Visibility = ViewStates.Gone; // hide warning message.
						TrackButton.Enabled = true;
					}

					/* Show temporary list. */
					TagListView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, listItems);
				});
			});
		}
	}
}

