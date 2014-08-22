using System;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "KinderFinder", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		EditText emailBox,
			passwordBox;
		CheckBox rememberMeBox;
		Button loginButton,
			registerButton;
		ProgressBar progressBar;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			emailBox = FindViewById<EditText>(Resource.Id.Main_Email);
			passwordBox = FindViewById<EditText>(Resource.Id.Main_Password);
			rememberMeBox = FindViewById<CheckBox>(Resource.Id.Main_Remember);
			loginButton = FindViewById<Button>(Resource.Id.Main_Login);
			registerButton = FindViewById<Button>(Resource.Id.Main_Register);
			progressBar = FindViewById<ProgressBar>(Resource.Id.Main_ProgressBar);

			loginButton.Click += LogInPressed;
			registerButton.Click += (sender, e) => StartActivity(new Intent(this, typeof(RegisterActivity)));

			string email = pref.GetString(Globals.KEY_USERNAME, "");
			string passwordHash = pref.GetString(Globals.KEY_PASSWORD_HASH, "");
			bool rememberMe = pref.GetBoolean(Globals.KEY_REMEMBER_ME, false);

			emailBox.Text = email;
			rememberMeBox.Checked = rememberMe;
			progressBar.Visibility = Android.Views.ViewStates.Invisible;

			/* If the user checked "Remember me"; auto-login. */
			if (rememberMe && !email.Equals("") && !passwordHash.Equals(""))
				LogIn(email, passwordHash);
		}

		/// <summary>
		/// Attempts to log the user in with the provided details. If successful, a new activity is started.
		/// </summary>
		/// <param name="email">User's email address.</param>
		/// <param name="passwordHash">Hash of user's password.</param>
		void LogIn(string email, string passwordHash) {
			string data = "{" +
			              "\"EmailAddress\":\"" + email + "\"," +
			              "\"PasswordHash\":\"" + passwordHash + "\"" +
			              "}";

			/* Disable buttons and show progress bar. */
			loginButton.Enabled = false;
			registerButton.Enabled = false;
			progressBar.Visibility = Android.Views.ViewStates.Visible;

			/* Send request in a separate thread. */
			ThreadPool.QueueUserWorkItem(state => {
				ServerResponse reply = Utility.SendData("api/login", data);
				string message = "";

				/* Check reply status code. */
				switch (reply.StatusCode) {
				/* Log in succeeded. */
					case HttpStatusCode.OK:
						message = "Logged in!";
						editor.PutString(Globals.KEY_USERNAME, email);
						editor.PutString(Globals.KEY_PASSWORD_HASH, passwordHash);
						editor.PutBoolean(Globals.KEY_REMEMBER_ME, rememberMeBox.Checked);
						editor.Commit();
						;
						StartActivity(new Intent(this, typeof(RestaurantListActivity)));
						Finish();
						break;
				/* Invalid details provided. */
					case HttpStatusCode.BadRequest:
						message = "Invalid details";
						break;
				/* Some kind of server error happened. */
					default:
						Console.WriteLine("Error: " + reply.StatusCode.ToString());
						message = "Server error. Please try again later";
						break;
				}

				/* Enable buttons and hide progress bar. Done on main thread. */
				RunOnUiThread(() => {
					Toast.MakeText(this, message, ToastLength.Long).Show();
					loginButton.Enabled = true;
					registerButton.Enabled = true;
					progressBar.Visibility = Android.Views.ViewStates.Invisible;
				});
			});
			//FindViewById<Button>(Resource.Id.Main_Login).Click += LogUserIn;
			//FindViewById<Button>(Resource.Id.Main_Register).Click += RegisterUser;
		}

		/// <summary>
		/// Executed when the Log In button is pressed. Checks whether a valid email address and password are provided
		/// and then attempts to log in.
		/// </summary>
		void LogInPressed(object sender, EventArgs e) {
			string email = emailBox.Text;
			string password = passwordBox.Text;

			/* Hide keyboard. */
			var manager = (InputMethodManager)GetSystemService(InputMethodService);
			manager.HideSoftInputFromWindow(emailBox.WindowToken, 0);

			/* Invalid email address. */
			if (!Utility.IsValidEmailAddress(email))
				Toast.MakeText(this, "Please enter a valid email address", ToastLength.Long).Show();
			/* Invalid password length. */
			else if (password.Length < Globals.PASSWORD_MIN_LENGTH || password.Length > Globals.PASSWORD_MAX_LENGTH)
				Toast.MakeText(this, "Password must be between " + Globals.PASSWORD_MIN_LENGTH + " and " + Globals.PASSWORD_MAX_LENGTH + " characters long", ToastLength.Long).Show();
			/* Valid details; attempt to log in. */
			else
				LogIn(email, Utility.HashPassword(password));
		}
	}
}
