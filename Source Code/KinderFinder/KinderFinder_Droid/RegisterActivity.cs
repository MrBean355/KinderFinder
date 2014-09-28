using System;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KinderFinder {

	[Activity(Label = "Register Account", Icon = "@drawable/icon")]
	public class RegisterActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		EditText firstNameBox,
			surnameBox,
			emailBox,
			phoneBox,
			passwordBox,
			passwordConfirmBox;
		Button registerButton;
		ProgressBar progressBar;

		public override bool OnCreateOptionsMenu(Android.Views.IMenu menu) {
			base.OnCreateOptionsMenu(menu);
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);

			int items = menu.Size();
			menu.GetItem(items - 2).SetEnabled(false); // disable log out item (second-last one).

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(Android.Views.IMenuItem item) {
			base.OnOptionsItemSelected(item);

			switch (item.ItemId) {
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

			pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			firstNameBox = FindViewById<EditText>(Resource.Id.Register_FirstName);
			surnameBox = FindViewById<EditText>(Resource.Id.Register_Surname);
			emailBox = FindViewById<EditText>(Resource.Id.Register_Email);
			phoneBox = FindViewById<EditText>(Resource.Id.Register_Phone);
			passwordBox = FindViewById<EditText>(Resource.Id.Register_Password);
			passwordConfirmBox = FindViewById<EditText>(Resource.Id.Register_PasswordConfirm);
			registerButton = FindViewById<Button>(Resource.Id.Register_Register);
			progressBar = FindViewById<ProgressBar>(Resource.Id.Register_ProgressBar);

			registerButton.Click += RegisterPressed;

			progressBar.Visibility = Android.Views.ViewStates.Invisible;
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
			registerButton.Enabled = false;
			progressBar.Visibility = Android.Views.ViewStates.Visible;

			/* Send request in a separate thread. */
			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/register", builder.ToString());
				string message;

				/* Check reply status code. */
				switch (reply.StatusCode) {
				/* Registration was successful. */
					case HttpStatusCode.OK:
						message = "Registration successful!";
						editor.PutString(Settings.Keys.USERNAME, email);
						editor.Remove(Settings.Keys.REMEMBER_ME);
						editor.Remove(Settings.Keys.PASSWORD_HASH);
						editor.Commit();
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
					registerButton.Enabled = true;
					progressBar.Visibility = Android.Views.ViewStates.Invisible;
				});
			});
		}

		/// <summary>
		/// Executed when the Register button is pressed. Validates the entered details then sends the details to the
		/// server to attempt to create a new account.
		/// </summary>
		void RegisterPressed(object sender, EventArgs e) {
			string firstName = firstNameBox.Text;
			string surname = surnameBox.Text;
			string email = emailBox.Text;
			string phone = phoneBox.Text;
			string pass1 = passwordBox.Text;
			string pass2 = passwordConfirmBox.Text;
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
