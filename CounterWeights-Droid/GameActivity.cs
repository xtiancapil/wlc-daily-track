
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
using Android.Support.V4.View;
using com.refractored;
using Android.Support.V4.App;

namespace CounterWeightsDroid
{
	[Activity (Label = "Counter Weights")]			
	public class GameActivity : ActionBarActivity
	{
		MyPagerAdapter adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.actvity_game);
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.toolbar);

			SetSupportActionBar (toolbar);

//			var transaction = SupportFragmentManager.BeginTransaction ();
//			transaction.Replace (Resource.Id.fragment_container, new OverviewFragment ());
//			transaction.Commit ();
//
			// Initialize the ViewPager and set an adapter
			adapter = new MyPagerAdapter(SupportFragmentManager);

			var pager =  FindViewById<ViewPager>(Resource.Id.pager);
			pager.Adapter = adapter;
			// Bind the tabs to the ViewPager
			var tabs = FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
			tabs.SetViewPager(pager);		}

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

		public class MyPagerAdapter : FragmentPagerAdapter{
			private  string[] Titles = { "Daily Play", "My Results", "Leaderboard" }; 
			//, "Top Free", "Top Grossing", "Top New Paid",
			//	"Top New Free", "Trending"};

			public MyPagerAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm)
			{
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted (int position)
			{
				return new Java.Lang.String (Titles [position]);
			}
			#region implemented abstract members of PagerAdapter
			public override int Count {
				get {
					return Titles.Length;
				}
			}
			#endregion
			#region implemented abstract members of FragmentPagerAdapter
			public override Android.Support.V4.App.Fragment GetItem (int position)
			{
				if (position == 0) {
					return ReflectionFeedFragment.NewInstance (position);
				} else if (position == 1) {
					return OverviewFragment.NewInstance (position);
				}

				return LeaderboardFragment.NewInstance (position);
			}
			#endregion
		}
	}
}

