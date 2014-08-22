using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics;

using System.Threading;

namespace KinderFinder_Droid {

	[Activity(Label = "Track Tags")]			
	public class TrackTagsActivity : Activity {
		ISharedPreferences pref;
		ISharedPreferencesEditor editor;
		ImageView mapImage;
		ProgressBar progressBar;
		TextView downloadingText;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TrackTags);

			pref = GetSharedPreferences(Globals.PREFERENCES_FILE, 0);
			editor = pref.Edit();

			mapImage = FindViewById<ImageView>(Resource.Id.Track_Map);
			progressBar = FindViewById<ProgressBar>(Resource.Id.Track_ProgressBar);
			downloadingText = FindViewById<TextView>(Resource.Id.Track_DownloadingText);

			LoadImage();
		}

		Bitmap ResizeBitmap(Bitmap input) {
			var screenWidth = Resources.DisplayMetrics.WidthPixels;
			var screenHeight = Resources.DisplayMetrics.HeightPixels;
			int newWidth, newHeight;
			double ratio;

			if (input.Width >= input.Height) {
				ratio = (double)screenWidth / input.Width;
				newWidth = screenWidth;
				newHeight = (int)(ratio * input.Height);
			}
			else {
				ratio = (double)screenHeight / input.Height;
				newWidth = (int)(ratio * input.Width);
				newHeight = screenHeight;
			}

			return Bitmap.CreateScaledBitmap(input, newWidth, newHeight, false);
		}

		void LoadImage() {
			// Hide map image:
			mapImage.Visibility = Android.Views.ViewStates.Gone;

			// Send request:
			ThreadPool.QueueUserWorkItem(state => {
				string data = "{" +
				              "\"EmailAddress\":\"" + pref.GetString(Globals.KEY_USERNAME, "") +
				              "\"}";

				var response = Utility.SendData("api/map", null);
				var bitmap = BitmapFactory.DecodeByteArray(response.Bytes, 0, response.Bytes.Length);
				var scaled = ResizeBitmap(bitmap);

				RunOnUiThread(() => {
					mapImage.SetImageBitmap(scaled);
					progressBar.Visibility = Android.Views.ViewStates.Gone;
					downloadingText.Visibility = Android.Views.ViewStates.Gone;
					mapImage.Visibility = Android.Views.ViewStates.Visible;
				});
			});

			// TODO: Save image on device.
		}
	}
}

