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

	[Activity(Label = "Register Account", Icon = "@drawable/icon")]
	public class RegisterActivity : Activity {
		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;
		EditText FirstNameBox,
			SurnameBox,
			EmailBox,
			PhoneBox,
			PasswordBox,
			PasswordConfirmBox;
		Button RegisterButton;
		ProgressBar ProgressBar;

		public override bool OnCreateOptionsMenu(IMenu menu) {
			base.OnCreateOptionsMenu(menu);
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);

			int items = menu.Size();
			menu.GetItem(items - 2).SetEnabled(false); // disable log out item.
			menu.GetItem(items - 3).SetEnabled(false); // disable edit details item.
			menu.GetItem(items - 4).SetEnabled(false); // disable change restaurant item.

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
			SetContentView(Resource.Layout.Register);

			Pref = GetSharedPreferences(Settings.Storage.PREFERENCES_FILE, 0);
			Editor = Pref.Edit();

			FirstNameBox = FindViewById<EditText>(Resource.Id.Register_FirstName);
			SurnameBox = FindViewById<EditText>(Resource.Id.Register_Surname);
			EmailBox = FindViewById<EditText>(Resource.Id.Register_Email);
			PhoneBox = FindViewById<EditText>(Resource.Id.Register_Phone);
			PasswordBox = FindViewById<EditText>(Resource.Id.Register_Password);
			PasswordConfirmBox = FindViewById<EditText>(Resource.Id.Register_PasswordConfirm);
			RegisterButton = FindViewById<Button>(Resource.Id.Register_Register);
			ProgressBar = FindViewById<ProgressBar>(Resource.Id.Register_ProgressBar);

			RegisterButton.Click += RegisterPressed;

			ProgressBar.Visibility = ViewStates.Invisible;
		}

		/// <summary>
		/// Attempts to register a new user with the provided details. If successful, ends the activity.
		/// </summary>
		/// <param name="firstName">User's first name.</param>
		/// <param name="surname">User's surname</param>
		/// <param name="email">User's email address.</param>
		/// <param name="phone">User's phone number.</param>
		/// <param name="passwordHash">Hash of user's password.</param>
		void Register(string firstName, string surname, string email, string phone, string passwordHash) {
			var builder = new JsonBuilder();
			builder.AddEntry("FirstName", firstName);
			builder.AddEntry("Surname", surname);
			builder.AddEntry("EmailAddress", email);
			builder.AddEntry("PhoneNumber", phone);
			builder.AddEntry("PasswordHash", passwordHash);

			/* Disable button and show progress bar. */
			RegisterButton.Enabled = false;
			ProgressBar.Visibility = ViewStates.Visible;

			/* Send request in a separate thread. */
			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/register", builder.ToString());
				string message;

				/* Check reply status code. */
				switch (reply.StatusCode) {
				/* Registration was successful. */
					case HttpStatusCode.OK:
						message = "Registration successful!";
						Editor.PutString(Settings.Keys.USERNAME, email);
						Editor.Remove(Settings.Keys.REMEMBER_ME);
						Editor.Remove(Settings.Keys.PASSWORD_HASH);
						Editor.Commit();
						Finish();
						break;
				/* Email address is already in use. */
					case HttpStatusCode.Conflict:
						message = "Email address already in use";
						break;
				/* Some kind of server error happened. */
					default:
						message = Settings.Errors.SERVER_ERROR;
						break;
				}

				/* Enable button and hide progress bar. Done on main thread. */
				RunOnUiThread(() => {
					Toast.MakeText(this, message, ToastLength.Short).Show();
					RegisterButton.Enabled = true;
					ProgressBar.Visibility = ViewStates.Invisible;
				});
			});
		}

		/// <summary>
		/// Executed when the Register button is pressed. Validates the entered details then sends the details to the
		/// server to attempt to create a new account.
		/// </summary>
		void RegisterPressed(object sender, EventArgs e) {
			string firstName = FirstNameBox.Text;
			string surname = SurnameBox.Text;
			string email = EmailBox.Text;
			string phone = PhoneBox.Text;
			string pass1 = PasswordBox.Text;
			string pass2 = PasswordConfirmBox.Text;
			string errorMsg = null;

			if (!Validator.IsValidName(firstName))
				errorMsg = string.Format("{0} must be a valid name between {1} and {2} characters", "First name", Settings.Lengths.NAME_MIN, Settings.Lengths.NAME_MAX);
			else if (!Validator.IsValidName(surname))
				errorMsg = string.Format("{0} must be a valid name between {1} and {2} characters", "Surname", Settings.Lengths.NAME_MIN, Settings.Lengths.NAME_MAX);
			else if (!Validator.IsValidEmailAddress(email))
				errorMsg = "Email address must be valid";
			else if (phone == "")
				errorMsg = "Please enter a phone number";
			else if (!Validator.IsValidPassword(pass1))
				errorMsg = string.Format("{0} must be between {1} and {2} characters", "Password", Settings.Lengths.PASSWORD_MIN, Settings.Lengths.PASSWORD_MAX);
			else if (pass1 != pass2)
				errorMsg = "Passwords do not match";

			if (errorMsg == null)
				Register(firstName, surname, email, phone, AppTools.HashPassword(pass1));
			else
				Toast.MakeText(this, errorMsg, ToastLength.Short).Show();
		}
	}
}
