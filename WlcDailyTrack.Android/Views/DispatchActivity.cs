
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WlcDailyTrackAndroid
{
	[Activity (Label = ".views.DispatchActivity", MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class DispatchActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			var serialized = prefs.GetString ("cookies", null);
			var username = prefs.GetString ("wlcUsername", null);
			var pass = prefs.GetString ("wlcPass", null);

			if (!string.IsNullOrEmpty(serialized) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(pass) ) {

				StartActivity (new Intent (this, typeof(ScoreActivity)));
			} else {
				//Start an intent for the logged out activity
				StartActivity (new Intent (this, typeof(SignInActivity)));
			}
			// Create your application here
		}
	}
}

