using System.Reflection;

using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;

namespace KinderFinder_DroidTests {

	[Activity(Label = "KinderFinder_DroidTests", MainLauncher = true)]
	public class MainActivity : TestSuiteActivity {

		protected override void OnCreate(Bundle bundle) {
			AddTest(Assembly.GetExecutingAssembly());
			base.OnCreate(bundle);
		}
	}
}

