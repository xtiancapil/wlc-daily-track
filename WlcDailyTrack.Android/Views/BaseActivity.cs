
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;

using Android.Support.V7.App;

namespace WlcDailyTrackAndroid
{
	[Android.App.Activity (Label = "BaseActivity")]			
	public class BaseActivity : ActionBarActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Window.RequestFeature (WindowFeatures.ActionBarOverlay);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			ConnectivityManager cm = (ConnectivityManager) GetSystemService (Context.ConnectivityService);
			NetworkInfo ni = cm.ActiveNetworkInfo;
			if ((ni == null) || (!ni.IsConnected)) {
				Toast.MakeText (ApplicationContext, Resource.String.device_offline_message, ToastLength.Long).Show ();
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater inflater = MenuInflater;
			inflater.Inflate (Resource.Menu.main, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.action_logout:
				Parse.ParseUser.LogOut ();
				var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
				var editor = prefs.Edit ();
				editor.Clear ();
				editor.Commit ();


				var intent = new Intent (this, typeof(DispatchActivity));
				intent.AddFlags (ActivityFlags.ClearTask | ActivityFlags.NewTask);
				StartActivity (intent);
				break;

			}
			return base.OnOptionsItemSelected (item);
		}
	}
}

