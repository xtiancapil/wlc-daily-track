
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
using Android.Support.V7.App;

namespace CounterWeightsDroid
{
	[Activity (Label = "Counter Weights")]			
	public class GameActivity : ActionBarActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.actvity_game);
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.toolbar);

			SetSupportActionBar (toolbar);

			var transaction = SupportFragmentManager.BeginTransaction ();
			transaction.Replace (Resource.Id.fragment_container, new OverviewFragment ());
			transaction.Commit ();

			// Create your application here
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.home, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_logout) {
				var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
				var editor = prefs.Edit ();
				editor.Clear ();
				editor.Commit ();

				var intent = new Intent (this, typeof(MainActivity));
				intent.AddFlags (ActivityFlags.ClearTask | ActivityFlags.NewTask);
				StartActivity (intent);
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
	}
}

