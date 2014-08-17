
using Android.App;
using Android.OS;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "TrackTagsActivity")]			
	public class TrackTagsActivity : Activity {
		ImageView mapImage;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TrackTags);

			mapImage = FindViewById<ImageView>(Resource.Id.Track_Map);

			LoadImage();
		}

		void LoadImage() {
			/*var response = Utility.SendData("api/map", null);

			Bitmap bmp = BitmapFactory.DecodeByteArray(response.Bytes, 0, 16 * 1024);

			if (bmp == null)
				Console.WriteLine("--> It's null");
			else
				Console.WriteLine("--> It's NOT null");

			mapImage.SetImageBitmap(bmp);*/
		}
	}
}

