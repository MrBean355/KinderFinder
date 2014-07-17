using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Threading;
using Android.Views.Animations;

namespace KinderFinder_Droid {

	[Activity(Label = "KinderFinder", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity {

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			FindViewById<Button>(Resource.Id.Main_Register).Click += (object sender, EventArgs e) => {
				Intent intent = new Intent(this, typeof(RegisterActivity));
				StartActivity(intent);
			};

			FindViewById<Button>(Resource.Id.Main_Login).Click += LogUserIn;
		}

		private void LogUserIn(object sender, EventArgs e) {
			string email = FindViewById<EditText>(Resource.Id.Main_Email).Text;
			string password = FindViewById<EditText>(Resource.Id.Main_Password).Text;

			if (!Utility.IsValidEmailAddress(email))
				Toast.MakeText(this, "Please enter a valid email address", ToastLength.Long).Show();
			else if (password.Length < 6 || password.Length > 50)
				Toast.MakeText(this, "Password must be between 6 and 50 characters long", ToastLength.Long).Show();
			else {
				Button login = FindViewById<Button>(Resource.Id.Main_Login);
				Button register = FindViewById<Button>(Resource.Id.Main_Register);
				string data = "{" +
		              "\"EmailAddress\":\"" + email + "\"," +
					"\"PasswordHash\":\"" + Utility.HashPassword(password) + "\"}";

				login.Enabled = false;
				register.Enabled = false;

				ThreadPool.QueueUserWorkItem(delegate(object state) {
					HttpStatusCode code = Utility.SendData("api/login", data);
					string message = "";

					if (code.Equals(HttpStatusCode.OK))
						message = "Logged in!";
					else if (code.Equals(HttpStatusCode.BadRequest))
						message = "Invalid email address or password";
					else
						message = "Server error. Please try again later";

					RunOnUiThread(delegate() {
						login.Enabled = true;
						register.Enabled = true;
						Toast.MakeText(this, message, ToastLength.Long).Show();
					});
				});
			}
		}
	}
}


