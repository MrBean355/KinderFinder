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

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			FindViewById<Button>(Resource.Id.Main_Login).Click += LogUserIn;
			FindViewById<Button>(Resource.Id.Main_Register).Click += RegisterUser;

		}

		/**
		 * Attempts to log the user in by contacting the server and checking their details.
		 */
		private void LogUserIn(object sender, EventArgs e) {
			EditText emailBox = FindViewById<EditText>(Resource.Id.Main_Email);
			string email = emailBox.Text;
			string password = FindViewById<EditText>(Resource.Id.Main_Password).Text;

			InputMethodManager manager = (InputMethodManager)GetSystemService(InputMethodService);
			manager.HideSoftInputFromWindow(emailBox.WindowToken, 0);

			/* Invalid email address. */
			if (!Utility.IsValidEmailAddress(email))
				Toast.MakeText(this, "Please enter a valid email address", ToastLength.Long).Show();
			/* Invalid password length. */
			else if (password.Length < Globals.PASSWORD_MIN_LENGTH || password.Length > Globals.PASSWORD_MAX_LENGTH)
				Toast.MakeText(this, "Password must be between " + Globals.PASSWORD_MIN_LENGTH + " and " + Globals.PASSWORD_MAX_LENGTH + " characters long", ToastLength.Long).Show();
			/* Valid details; send request. */
			else {
				Button login = FindViewById<Button>(Resource.Id.Main_Login);
				Button register = FindViewById<Button>(Resource.Id.Main_Register);
				string data = "{" +
				              "\"EmailAddress\":\"" + email + "\"," +
				              "\"PasswordHash\":\"" + Utility.HashPassword(password) + "\"}";

				/* Disable Log In and Register buttons. */
				login.Enabled = false;
				register.Enabled = false;

				/* Contact server in separate thread. */
				ThreadPool.QueueUserWorkItem(delegate(object state) {
					ServerResponse reply = Utility.SendData("api/login", data);
					string message = "";
					bool success = false;

					/* Log in was successful. */
					if (reply.StatusCode.Equals(HttpStatusCode.OK)) {
						message = "Logged in!";
						success = true;
					}
					/* Invalid details. */
					else if (reply.StatusCode.Equals(HttpStatusCode.BadRequest))
						message = "Invalid email address or password";
					/* Some other error. */
					else
						message = "Server error. Please try again later";

					/* Update UI. */
					RunOnUiThread(delegate() {
						/* Enable buttons and display message. */
						login.Enabled = true;
						register.Enabled = true;
						Toast.MakeText(this, message, ToastLength.Long).Show();

						/* Display list of tags if logged in. */
						if (success) {
							ISharedPreferences pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
							ISharedPreferencesEditor editor = pref.Edit();
							editor.PutString(Globals.KEY_USERNAME, email);
							editor.Commit();

							Intent intent = new Intent(this, typeof(TagListActivity));
							StartActivity(intent);
							Finish();
						}
					});
				});
			}
		}

		/**
		 * Starts a new activity for the user to create an account.
		 */
		private void RegisterUser(object sender, EventArgs e) {
			StartActivity(new Intent(this, typeof(RegisterActivity)));
		}
	}
}


