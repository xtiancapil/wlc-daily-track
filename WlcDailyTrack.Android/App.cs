using System;
using Android.App;
using Android.Runtime;
using Parse;

namespace WlcDailyTrackAndroid
{
	[Application]
	public class App : Application
	{
		public App (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();

			// Initialize the parse client with your Application ID and .NET Key found on
			// your Parse dashboard
			ParseClient.Initialize( GetString(Resource.String.parse_application_id),
				GetString(Resource.String.parse_client_key));
		}
	}
}

