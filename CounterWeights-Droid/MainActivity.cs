using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

namespace CounterWeightsDroid
{
	[Activity (MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : ActionBarActivity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			var serialized = prefs.GetString ("cookies", null);
			var username = prefs.GetString ("wlcUsername", null);
			var pass = prefs.GetString ("wlcPass", null);

			if (!string.IsNullOrEmpty(serialized) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(pass) ) {
				StartActivity (new Intent (this, typeof(GameActivity)));
			} else {
				//Start an intent for the logged out activity
				StartActivity (new Intent (this, typeof(SignInActivity)));
			}
		}
	}
}


