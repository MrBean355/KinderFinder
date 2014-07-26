using System;
using System.Net;
using System.Threading;

using Android.App;
using Android.OS;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "Register", Icon = "@drawable/icon")]
	public class RegisterActivity : Activity {

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Register);

			FindViewById<Button>(Resource.Id.Register_Register).Click += RegisterUser;
		}

		private void RegisterUser(object sender, EventArgs e) {
			string firstName = FindViewById<EditText>(Resource.Id.Register_FirstName).Text;
			string surname = FindViewById<EditText>(Resource.Id.Register_Surname).Text;
			string email = FindViewById<EditText>(Resource.Id.Register_Email).Text;
			string pass1 = FindViewById<EditText>(Resource.Id.Register_Password).Text;
			string pass2 = FindViewById<EditText>(Resource.Id.Register_PasswordConfirm).Text;

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
			/* All details are valid. */
			else {
				/* Disable Register button. */
				Button register = FindViewById<Button>(Resource.Id.Register_Register);
				register.Enabled = false;

				/* Construct data string. */
				string data = "{" +
				              "\"FirstName\":\"" + firstName + "\"," +
				              "\"Surname\":\"" + surname + "\"," +
				              "\"EmailAddress\":\"" + email + "\"," +
				              "\"PasswordHash\":\"" + Utility.HashPassword(pass1) + "\"}";

				/* Contact server in separate thread. */
				ThreadPool.QueueUserWorkItem(delegate (object state) {
					ServerResponse reply = Utility.SendData("api/register", data);
					string message = "";

					/* Registration was successful. */
					if (reply.StatusCode.Equals(HttpStatusCode.OK)) {
						message = "Registration successful!";
						Finish();
					}
					/* Email address in use. */
					else if (reply.StatusCode.Equals(HttpStatusCode.Conflict))
						message = "Email address is already in use";
					/* Some other error. */
					else
						message = "Server error. Please try again later";

					/* Update UI. */
					RunOnUiThread(delegate() {
						register.Enabled = true;
						Toast.MakeText(this, message, ToastLength.Long).Show();
					});
				});
			}
		}

	}
}

