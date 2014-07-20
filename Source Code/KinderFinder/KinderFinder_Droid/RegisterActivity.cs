
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace KinderFinder_Droid {

	[Activity(Label = "Register")]			
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
			if (firstName.Length < 3 || firstName.Length > 50)
				Toast.MakeText(this, "First name must be between 3 and 50 characters long", ToastLength.Long).Show();
			else if (surname.Length < 3 || surname.Length > 50)
				Toast.MakeText(this, "Surname must be between 3 and 50 characters long", ToastLength.Long).Show();
			else if (!Utility.IsValidEmailAddress(email))
				Toast.MakeText(this, "Please enter a valid email address", ToastLength.Long).Show();
			else if (pass1.Length < 6 || pass1.Length > 50)
				Toast.MakeText(this, "Password must be between 6 and 50 characters long", ToastLength.Long).Show();
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

