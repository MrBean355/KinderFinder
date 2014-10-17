using System;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using KinderFinder.Utility;

namespace KinderFinder {

	[Activity(Label = "KinderFinder", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity {
		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;
		EditText EmailBox,
			PasswordBox;
		CheckBox RememberMeBox;
		Button LoginButton,
			RegisterButton;
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
			SetContentView(Resource.Layout.Main);

			Pref = GetSharedPreferences(Settings.PREFERENCES_FILE, 0);
			Editor = Pref.Edit();

			EmailBox = FindViewById<EditText>(Resource.Id.Main_Email);
			PasswordBox = FindViewById<EditText>(Resource.Id.Main_Password);
			RememberMeBox = FindViewById<CheckBox>(Resource.Id.Main_Remember);
			LoginButton = FindViewById<Button>(Resource.Id.Main_Login);
			RegisterButton = FindViewById<Button>(Resource.Id.Main_Register);
			ProgressBar = FindViewById<ProgressBar>(Resource.Id.Main_ProgressBar);

			LoginButton.Click += LogInPressed;
			RegisterButton.Click += (sender, e) => StartActivity(new Intent(this, typeof(RegisterActivity)));
		}

		protected override void OnResume() {
			base.OnResume();

			string email = Pref.GetString(Settings.Keys.USERNAME, null);
			string passwordHash = Pref.GetString(Settings.Keys.PASSWORD_HASH, null);
			bool rememberMe = Pref.GetBoolean(Settings.Keys.REMEMBER_ME, false);
			ProgressBar.Visibility = ViewStates.Invisible;

			/* Was able to load email and password hash. */
			if (email != null && passwordHash != null) {
				EmailBox.Text = email;
				RememberMeBox.Checked = rememberMe;

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
			LoginButton.Enabled = false;
			RegisterButton.Enabled = false;
			ProgressBar.Visibility = ViewStates.Visible;

			/* Send request in a separate thread. */
			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/login", builder.ToString());
				string message;

				/* Check reply status code. */
				switch (reply.StatusCode) {
				/* Log in succeeded. */
					case HttpStatusCode.OK:
						message = "Logged in!";
						string prevUser = Pref.GetString(Settings.Keys.USERNAME, null);

						/* Clear previous local data. */
						if (prevUser != null && prevUser != email) {
							Editor.Clear();
							Editor.Commit();
						}

						Editor.PutString(Settings.Keys.USERNAME, email);
						Editor.PutString(Settings.Keys.PASSWORD_HASH, passwordHash);
						Editor.PutBoolean(Settings.Keys.REMEMBER_ME, RememberMeBox.Checked);
						Editor.Commit();

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
					LoginButton.Enabled = true;
					RegisterButton.Enabled = true;
					ProgressBar.Visibility = ViewStates.Invisible;
				});
			});
		}

		/// <summary>
		/// Executed when the Log In button is pressed. Checks whether a valid email address and password are provided
		/// and then attempts to log in.
		/// </summary>
		void LogInPressed(object sender, EventArgs e) {
			string email = EmailBox.Text;
			string password = PasswordBox.Text;
			string errorMsg = null;

			/* Hide keyboard. */
			var manager = (InputMethodManager)GetSystemService(InputMethodService);
			manager.HideSoftInputFromWindow(EmailBox.WindowToken, 0);

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
