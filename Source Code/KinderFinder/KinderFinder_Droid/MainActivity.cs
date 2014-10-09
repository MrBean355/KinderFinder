using System;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;

using KinderFinder.Utility;
using Android.Media;

namespace KinderFinder {

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
					System.Environment.Exit(0); // TODO: Find a better way to do this.
					break;
				default:
					Toast.MakeText(this, "Unknown menu item selected", ToastLength.Short).Show();
					break;
			}

			return base.OnOptionsItemSelected(item);
		}

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			emailBox = FindViewById<EditText>(Resource.Id.Main_Email);
			passwordBox = FindViewById<EditText>(Resource.Id.Main_Password);
			rememberMeBox = FindViewById<CheckBox>(Resource.Id.Main_Remember);
			loginButton = FindViewById<Button>(Resource.Id.Main_Login);
			registerButton = FindViewById<Button>(Resource.Id.Main_Register);
			progressBar = FindViewById<ProgressBar>(Resource.Id.Main_ProgressBar);

			loginButton.Click += LogInPressed;
			registerButton.Click += (sender, e) => StartActivity(new Intent(this, typeof(RegisterActivity)));
		}

		protected override void OnResume() {
			base.OnResume();

			string email = pref.GetString(Settings.Keys.USERNAME, null);
			string passwordHash = pref.GetString(Settings.Keys.PASSWORD_HASH, null);
			bool rememberMe = pref.GetBoolean(Settings.Keys.REMEMBER_ME, false);
			progressBar.Visibility = Android.Views.ViewStates.Gone;

			/* Was able to load email and password hash. */
			if (email != null && passwordHash != null) {
				emailBox.Text = email;
				rememberMeBox.Checked = rememberMe;

				/* If "Remember Me" was ticked, try to log in: */
				if (rememberMe)
					LogIn(email, passwordHash);
			}
		}

		/// <summary>
		/// Attempts to log the user in with the provided details. If successful, a new activity is started.
		/// </summary>
		/// <param name="email">User's email address.</param>
		/// <param name="passwordHash">Hash of user's password.</param>
		void LogIn(string email, string passwordHash) {
			var builder = new JsonBuilder();
			builder.AddEntry("EmailAddress", email);
			builder.AddEntry("PasswordHash", passwordHash);

			/* Disable buttons and show progress bar. */
			loginButton.Enabled = false;
			registerButton.Enabled = false;
			progressBar.Visibility = Android.Views.ViewStates.Visible;

			/* Send request in a separate thread. */
			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/login", builder.ToString());
				string message;

				/* Check reply status code. */
				switch (reply.StatusCode) {
				/* Log in succeeded. */
					case HttpStatusCode.OK:
						message = "Logged in!";
						string prevUser = pref.GetString(Settings.Keys.USERNAME, null);

						/* Clear previous local data. */
						if (prevUser != null && prevUser != email) {
							editor.Clear();
							editor.Commit();
						}

						editor.PutString(Settings.Keys.USERNAME, email);
						editor.PutString(Settings.Keys.PASSWORD_HASH, passwordHash);
						editor.PutBoolean(Settings.Keys.REMEMBER_ME, rememberMeBox.Checked);
						editor.Commit();

						StartActivity(new Intent(this, typeof(RestaurantListActivity)));
						Finish();
						break;
				/* Invalid details provided. */
					case HttpStatusCode.BadRequest:
						message = "Invalid details";
						break;
				/* Some kind of server error happened. */
					default:
						Console.WriteLine("Error: " + reply.StatusCode);
						message = Settings.Errors.SERVER_ERROR;
						break;
				}

				/* Enable buttons and hide progress bar. Done on main thread. */
				RunOnUiThread(() => {
					Toast.MakeText(this, message, ToastLength.Short).Show();
					loginButton.Enabled = true;
					registerButton.Enabled = true;
					progressBar.Visibility = Android.Views.ViewStates.Invisible;
				});
			});
		}

		/// <summary>
		/// Executed when the Log In button is pressed. Checks whether a valid email address and password are provided
		/// and then attempts to log in.
		/// </summary>
		void LogInPressed(object sender, EventArgs e) {
			string email = emailBox.Text;
			string password = passwordBox.Text;
			string errorMsg = null;

			/* Hide keyboard. */
			var manager = (InputMethodManager)GetSystemService(InputMethodService);
			manager.HideSoftInputFromWindow(emailBox.WindowToken, 0);

			/* Invalid email address. */
			if (!Validator.IsValidEmailAddress(email))
				errorMsg = "Email address must be valid";
			/* Invalid password length. */
			else if (!Validator.IsValidPassword(password))
				errorMsg = string.Format("{0} must be between {1} and {2} characters", "Password", Settings.Lengths.PASSWORD_MIN, Settings.Lengths.PASSWORD_MAX);

			if (errorMsg == null)
				LogIn(email, AppTools.HashPassword(password));
			else
				Toast.MakeText(this, errorMsg, ToastLength.Short).Show();
		}
	}
}
