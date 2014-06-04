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
			ParseClient.Initialize("yiQg1uGDQLaoFdhpebcRgLZVYiROJ8LgaEC18hAo",
				"rqGZe71Gx5w5y6dPhyZhMyx91N8wd3Tj90oC6df9");
		}
	}
}

