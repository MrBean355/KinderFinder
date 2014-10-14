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
		Spinner RestaurantList, TypeList;
		EditText XPosBox, YPosBox;
		Button TransmitButton;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			RestaurantList = FindViewById<Spinner>(Resource.Id.RestaurantList);
			TypeList = FindViewById<Spinner>(Resource.Id.TypeList);
			XPosBox = FindViewById<EditText>(Resource.Id.XPos);
			YPosBox = FindViewById<EditText>(Resource.Id.YPos);
			TransmitButton = FindViewById<Button>(Resource.Id.Transmit);

			RestaurantList.ItemSelected += (sender, e) => LoadTypes(RestaurantList.SelectedItem.ToString());
			TransmitButton.Click += TransmitPressed;

			LoadRestaurants();
		}

		void LoadTypes(string restaurant) {
			var jb = new JsonBuilder();
			jb.AddEntry("RestaurantName", restaurant);

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/transmittertype", jb.ToString());

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						var list = Deserialiser<List<string>>.Run(reply.Body);
						var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, list);
						adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
						RunOnUiThread(() => TypeList.Adapter = adapter);

						if (list.Count > 0) {
							RunOnUiThread(() => {
								TypeList.Enabled = true;
								XPosBox.Enabled = true;
								YPosBox.Enabled = true;
								TransmitButton.Enabled = true;
							});
						}

						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}
			});
		}

		void LoadRestaurants() {
			RestaurantList.Enabled = false;
			TypeList.Enabled = false;
			XPosBox.Enabled = false;
			YPosBox.Enabled = false;
			TransmitButton.Enabled = false;

			ThreadPool.QueueUserWorkItem(state => {
				var reply = AppTools.SendRequest("api/restaurantlist", null);

				switch (reply.StatusCode) {
					case HttpStatusCode.OK:
						var list = Deserialiser<List<string>>.Run(reply.Body);
						var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, list);
						adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
						RunOnUiThread(() => RestaurantList.Adapter = adapter);

						if (list.Count > 0)
							RunOnUiThread(() => RestaurantList.Enabled = true);

						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}
			});
		}

		void TransmitPressed(object sender, EventArgs e) {
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
						var intent = new Intent(this, typeof(TransmitActivity));
						intent.PutExtra("ID", reply.Body);
						StartActivity(intent);
						Finish();
						break;
					default:
						RunOnUiThread(() => Toast.MakeText(this, "Error contacting server", ToastLength.Short).Show());
						break;
				}
			});
		}
	}
}
