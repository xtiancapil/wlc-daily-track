using System;
using Android.App;
using Android.Runtime;
using Parse;
using ReactiveUI;

namespace WlcDailyTrackAndroid
{
	[Application]
	public class App : Application
	{
		AutoSuspendHelper suspendHelper;
		public App (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();

//			suspendHelper = new AutoSuspendHelper(this);
//			RxApp.SuspensionHost.CreateNewAppState = () => new AppBootstrapper();
//			RxApp.SuspensionHost.SetupDefaultSuspendResume();

			//Bootstrap any services we need
			var current = TinyIoC.TinyIoCContainer.Current;
			current.Register<Core.WlcWebService> (new Core.WlcWebService ());

			// Initialize the parse client with your Application ID and .NET Key found on
			// your Parse dashboard
			ParseClient.Initialize( GetString(Resource.String.parse_application_id),
				GetString(Resource.String.parse_client_key));
		}
	}
}

