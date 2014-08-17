using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics;

using System.Threading;
using Java.IO;

namespace KinderFinder_Droid {

	[Activity(Label = "TrackTagsActivity")]			
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

		void LoadImage() {
			var first = Utility.SendData("api/mapsize", null);
			string size = first.Body;
			string curSize = pref.GetString(Globals.KEY_MAP_SIZE, "");

			if (size.Equals("")) {
				Toast.MakeText(this, "Error retrieving map from server", ToastLength.Short).Show();
				return;
			}

			/* My map and server's map differ in size; download new one. */
			if (!curSize.Equals(size)) {
				mapImage.Visibility = Android.Views.ViewStates.Gone;

				ThreadPool.QueueUserWorkItem(state => {
					var response = Utility.SendData("api/map", null);
					var bitmap = BitmapFactory.DecodeByteArray(response.Bytes, 0, response.Bytes.Length);
					// TODO: Save map onto device.
					editor.PutString(Globals.KEY_MAP_SIZE, response.Bytes.Length + "");
					editor.Commit();

					RunOnUiThread(() => {
						mapImage.SetImageBitmap(bitmap);
						progressBar.Visibility = Android.Views.ViewStates.Gone;
						downloadingText.Visibility = Android.Views.ViewStates.Gone;
						mapImage.Visibility = Android.Views.ViewStates.Visible;
					});
				});
			}
		}
	}
}

