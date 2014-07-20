using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace KinderFinder_Droid {

	[Activity(Label = "TagListActivity")]			
	public class TagListActivity : ListActivity {
		List<string> Items;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			loadItems();
		}

		/**
		 * Fetches a list of tags linked to the user and displays them in a list.
		 */
		private void loadItems() {
			ServerResponse reply = Utility.SendData("api/taglist", "{ \"EmailAddress\":\"mrbean355@gmail.com\" }");
			Items = Utility.ParseJSON(reply.Body);
			ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, Items);
		}

		protected override void OnListItemClick(ListView list, View view, int position, long id) {
			string text = Items[position];
			Toast.MakeText(this, text, ToastLength.Short).Show();
		}
	}
}

