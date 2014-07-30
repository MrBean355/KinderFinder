using System;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "Register", Icon = "@drawable/icon")]
	public class RegisterActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		EditText firstNameBox,
			surnameBox,
			emailBox,
			passwordBox,
			passwordConfirmBox;
		Button registerButton;
		ProgressBar progressBar;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Register);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			firstNameBox = FindViewById<EditText>(Resource.Id.Register_FirstName);
			surnameBox = FindViewById<EditText>(Resource.Id.Register_Surname);
			emailBox = FindViewById<EditText>(Resource.Id.Register_Email);
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
		/// <param name="passwordHash">Hash of user's password.</param>
		void Register(string firstName, string surname, string email, string passwordHash) {
			string data = "{" +
			              "\"FirstName\":\"" + firstName + "\"," +
			              "\"Surname\":\"" + surname + "\"," +
			              "\"EmailAddress\":\"" + email + "\"," +
			              "\"PasswordHash\":\"" + passwordHash + "\"" +
			              "}";

			/* Disable button and show progress bar. */
			registerButton.Enabled = false;
			progressBar.Visibility = Android.Views.ViewStates.Visible;

			/* Send request in a separate thread. */
			ThreadPool.QueueUserWorkItem(state => {
				ServerResponse reply = Utility.SendData("api/register", data);
				string message = "";

				/* Check reply status code. */
				switch (reply.StatusCode) {
				/* Registration was successful. */
					case HttpStatusCode.OK:
						editor.Remove(Globals.KEY_REMEMBER_ME);
						editor.Remove(Globals.KEY_PASSWORD_HASH);
						editor.Commit();
						message = "Registration successful!";
						Finish();
						break;
				/* Email address is already in use. */
					case HttpStatusCode.Conflict:
						message = "Email address already in use";
						break;
				/* Some kind of server error happened. */
					default:
						message = "Server error. Please try again later";
						break;
				}

				/* Enable button and hide progress bar. Done on main thread. */
				RunOnUiThread(() => {
					Toast.MakeText(this, message, ToastLength.Long).Show();
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
			string pass1 = passwordBox.Text;
			string pass2 = passwordConfirmBox.Text;

			/* Validate provided details. */
			if (firstName.Length < Globals.NAME_MIN_LENGTH || firstName.Length > Globals.NAME_MAX_LENGTH)
				Toast.MakeText(this, string.Format("First name must be between {0} and {1} characters long", Globals.NAME_MIN_LENGTH, Globals.NAME_MAX_LENGTH), ToastLength.Long).Show();
			else if (surname.Length < Globals.NAME_MIN_LENGTH || surname.Length > Globals.NAME_MAX_LENGTH)
				Toast.MakeText(this, string.Format("Surname must be between {0} and {1} characters long", Globals.NAME_MIN_LENGTH, Globals.NAME_MAX_LENGTH), ToastLength.Long).Show();
			else if (!Utility.IsValidEmailAddress(email))
				Toast.MakeText(this, "Please enter a valid email address", ToastLength.Long).Show();
			else if (pass1.Length < Globals.PASSWORD_MIN_LENGTH || pass1.Length > Globals.PASSWORD_MAX_LENGTH)
				Toast.MakeText(this, string.Format("Password must be between {0} and {1} characters long", Globals.PASSWORD_MIN_LENGTH, Globals.PASSWORD_MAX_LENGTH), ToastLength.Long).Show();
			else if (!pass1.Equals(pass2))
				Toast.MakeText(this, "Passwords do not match", ToastLength.Long).Show();
			/* All details are valid; send register request. */
			else
				Register(firstName, surname, email, Utility.HashPassword(pass1));
		}
	}
}
