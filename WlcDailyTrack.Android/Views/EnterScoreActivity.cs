
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
using ReactiveUI;
using WlcDailyTracker.Core;

namespace WlcDailyTrackAndroid
{
	[Activity (Label = "EnterScoreActivity")]			
	public class EnterScoreActivity : ReactiveActivity<MainViewModel>
	{
		public EnterScoreActivity () {}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.WireUpControls ();
			ViewModel = new MainViewModel ();
			// Create your application here
			SetContentView (Resource.Layout.activity_enter_score);
		}
	}
}

