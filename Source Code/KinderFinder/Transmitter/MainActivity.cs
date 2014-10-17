using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using Transmitter.Utility;

namespace Transmitter {

	[Activity(Label = "Transmitter", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity {
		ISharedPreferences Pref;
		ISharedPreferencesEditor Editor;

		Spinner RestaurantList, TypeList;
		EditText XPosBox, YPosBox;
		Button StartButton, StopButton;
		ProgressBar ProgressBar;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			Pref = GetSharedPreferences("Preferences", 0);
			Editor = Pref.Edit();

			RestaurantList = FindViewById<Spinner>(Resource.Id.RestaurantList);
			TypeList = FindViewById<Spinner>(Resource.Id.TypeList);
			XPosBox = FindViewById<EditText>(Resource.Id.XPos);
			YPosBox = FindViewById<EditText>(Resource.Id.YPos);
			StartButton = FindViewById<Button>(Resource.Id.Start);
			StopButton = FindViewById<Button>(Resource.Id.Stop);
			ProgressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);

			RestaurantList.ItemSelected += (sender, e) => LoadTypes(RestaurantList.SelectedItem.ToString());
			StartButton.Click += StartPressed;
			StopButton.Click += StopPressed;

			CheckService();
		}

		void CheckService() {
			if (TransmitService.IsRunning()) {
				RestaurantList.Enabled = false;
				TypeList.Enabled = false;
				XPosBox.Enabled = false;
				YPosBox.Enabled = false;
				StartButton.Enabled = false;
				StopButton.Enabled = true;
			}
			else {
				RestaurantList.Enabled = false;
				TypeList.Enabled = false;
				XPosBox.Enabled = false;
				YPosBox.Enabled = false;
				StartButton.Enabled = false;
				StopButton.Enabled = false;
				LoadRestaurants();
			}
		}

		void LoadRestaurants() {
			ProgressBar.Visibility = Android.Views.ViewStates.Visible;

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/restaurantlist", null);

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						var list = Deserialiser<List<string>>.Run(reply.Body);
						var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, list);
						adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

						RunOnUiThread(() => {
							RestaurantList.Adapter = adapter;

							if (list.Count > 0)
								RestaurantList.Enabled = true;
						});

						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}

				RunOnUiThread(() => ProgressBar.Visibility = Android.Views.ViewStates.Invisible);
			});
		}

		void LoadTypes(string restaurant) {
			ProgressBar.Visibility = Android.Views.ViewStates.Visible;
			var jb = new JsonBuilder();
			jb.AddEntry("RestaurantName", restaurant);

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/transmittertype", jb.ToString());

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						var list = Deserialiser<List<string>>.Run(reply.Body);
						var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, list);
						adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

						RunOnUiThread(() => {
							TypeList.Adapter = adapter;

							if (list.Count > 0) {
								RunOnUiThread(() => {
									TypeList.Enabled = true;
									XPosBox.Enabled = true;
									YPosBox.Enabled = true;
									StartButton.Enabled = true;
								});
							}
						});

						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}

				RunOnUiThread(() => ProgressBar.Visibility = Android.Views.ViewStates.Invisible);
			});
		}

		void StartPressed(object sender, EventArgs e) {
			string restaurant = RestaurantList.SelectedItem.ToString();
			string type = TypeList.SelectedItem.ToString();
			string xPos = XPosBox.Text;
			string yPos = YPosBox.Text;

			if (restaurant == "" || type == "") {
				Toast.MakeText(this, "Something went wrong; please try again", ToastLength.Short).Show();
				return;
			}
			else if (xPos == "" || yPos == "") {
				Toast.MakeText(this, "Please enter x and y co-ords", ToastLength.Short).Show();
				return;
			}

			var jb = new JsonBuilder();
			jb.AddEntry("RestaurantName", restaurant);
			jb.AddEntry("TransmitterType", type);
			jb.AddEntry("X", xPos);
			jb.AddEntry("Y", yPos);

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/assignrestaurant", jb.ToString());

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						Editor.PutString("ID", reply.Body);
						Editor.Commit();
						StartService(new Intent(this, typeof(TransmitService)));
						RunOnUiThread(CheckService);
						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}
			});
		}

		void StopPressed(object sender, EventArgs e) {
			StopService(new Intent(this, typeof(TransmitService)));
			CheckService();
		}
	}
}
