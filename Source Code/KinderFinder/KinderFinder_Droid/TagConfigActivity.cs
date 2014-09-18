
using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using Java.Util;

namespace KinderFinder {

	[Activity(Label = "Configure Tag")]			
	public class TagConfigActivity : Activity {

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TagConfig);

			var spinner = FindViewById<Spinner>(Resource.Id.TagConfig_Colour);
			var items = new List<string>();
			items.Add("Hello");
			items.Add("World");
			items.Add("Bye bye");

			//spinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, items);
			spinner.Adapter = new ArrayAdapter(this, Resource.Layout.spinner_item, items);

			Toast.MakeText(this, "Selected: " + spinner.Adapter.GetItem(0), ToastLength.Short).Show();
			var item = spinner.GetChildAt(0);
			System.Console.WriteLine("-> " + item.Class);
		}
	}
}

