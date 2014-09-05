
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WlcDailyTrackAndroid
{
	public class DashboardFragment : Android.Support.V4.App.Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Get the data here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View v = inflater.Inflate (Resource.Layout.fragment_dashboard, container, false);

//			v.FindViewById<TextView> (Resource.Id.greeting).SetText ( (GetString (Resource.String.welcome_message, Parse.ParseUser.CurrentUser.Username)), TextView.BufferType.Normal);
			v.FindViewById<TextView> (Resource.Id.greeting).Text = "Welcome";
			return v;
		}

		private class ViewHolder {
			public TextView welcomeTitle { get; set; }
			public TextView welcomeContent { get; set; }
		}
	}
}

