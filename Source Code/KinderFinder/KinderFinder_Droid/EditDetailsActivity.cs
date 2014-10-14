using System;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "Edit Details", Icon = "@drawable/icon")]			
	public class EditDetailsActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		EditText firstNameBox,
			surnameBox,
			emailBox,
			phoneBox,
			passwordBox,
			passwordConfirmBox;
		Button cancelButton,
			saveButton;
		ProgressBar progressBar;

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
			SetContentView(Resource.Layout.EditDetails);

			pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			firstNameBox = FindViewById<EditText>(Resource.Id.Edit_FirstName);
			surnameBox = FindViewById<EditText>(Resource.Id.Edit_Surname);
			emailBox = FindViewById<EditText>(Resource.Id.Edit_Email);
			phoneBox = FindViewById<EditText>(Resource.Id.Edit_Phone);
			passwordBox = FindViewById<EditText>(Resource.Id.Edit_Password);
			passwordConfirmBox = FindViewById<EditText>(Resource.Id.Edit_PasswordConfirm);
			cancelButton = FindViewById<Button>(Resource.Id.Edit_Cancel);
			saveButton = FindViewById<Button>(Resource.Id.Edit_Save);
			progressBar = FindViewById<ProgressBar>(Resource.Id.Edit_ProgressBar);

			cancelButton.Click += CancelPressed;
			saveButton.Click += SavePressed;

			LoadDetails();
		}

		struct Details {
			public string FirstName,
				Surname,
				PhoneNumber;
		}

		void LoadDetails() {
			string email = pref.GetString(Settings.Keys.USERNAME, "");

			if (email.Equals("")) {
				Finish();
				return;
			}

			var builder = new JsonBuilder();
			builder.AddEntry("EmailAddress", email);
			progressBar.Visibility = ViewStates.Visible;

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/getdetails", builder.ToString());

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						var details = Deserialiser<Details>.Run(reply.Body);

						RunOnUiThread(() => {
							firstNameBox.Text = details.FirstName;
							surnameBox.Text = details.Surname;
							emailBox.Text = email;
							phoneBox.Text = details.PhoneNumber;
						});

						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}

				RunOnUiThread(() => progressBar.Visibility = ViewStates.Gone);
			});
		}

		void CancelPressed(object sender, EventArgs e) {
			Finish();
		}

		void SavePressed(object sender, EventArgs e) {
			string firstName = firstNameBox.Text;
			string surname = surnameBox.Text;
			string email = emailBox.Text;
			string phone = phoneBox.Text;
			string pass1 = passwordBox.Text;
			string pass2 = passwordConfirmBox.Text;
			bool newPassword = !pass1.Equals("");
			string errorMsg = null;

			if (!Validator.IsValidName(firstName))
				errorMsg = string.Format("{0} must be a valid name between {1} and {2} characters", "First name", Settings.Lengths.NAME_MIN, Settings.Lengths.NAME_MAX);
			else if (!Validator.IsValidName(surname))
				errorMsg = string.Format("{0} must be a valid name between {1} and {2} characters", "Surname", Settings.Lengths.NAME_MIN, Settings.Lengths.NAME_MAX);
			else if (!Validator.IsValidEmailAddress(email))
				errorMsg = "Email address must be valid";
			else if (phone == "")
				errorMsg = "Please enter a phone number";
			else if (newPassword && !Validator.IsValidPassword(pass1))
				errorMsg = string.Format("{0} must be between {1} and {2} characters", "Password", Settings.Lengths.PASSWORD_MIN, Settings.Lengths.PASSWORD_MAX);
			else if (newPassword && pass1 != pass2)
				errorMsg = "Passwords do not match";

			if (errorMsg != null) {
				Toast.MakeText(this, errorMsg, ToastLength.Short).Show();
				return;
			}

			string oldEmail = pref.GetString(Settings.Keys.USERNAME, "");

			if (oldEmail.Equals(""))
				return;

			var builder = new JsonBuilder();
			builder.AddEntry("FirstName", firstName);
			builder.AddEntry("Surname", surname);
			builder.AddEntry("OldEmailAddress", oldEmail);
			builder.AddEntry("EmailAddress", email);
			builder.AddEntry("PhoneNumber", phone);

			if (newPassword)
				builder.AddEntry("PasswordHash", AppTools.HashPassword(pass1));
			else
				builder.AddEntry("PasswordHash", "");

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/editdetails", builder.ToString());

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						RunOnUiThread(() => Toast.MakeText(this, "Details updated", ToastLength.Short).Show());
						Finish();
						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}

				RunOnUiThread(() => progressBar.Visibility = ViewStates.Gone);
			});
		}
	}
}

