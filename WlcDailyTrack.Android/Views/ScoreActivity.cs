
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
using Android.Graphics;
using Android.Support.V4.App;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.View;
using Android.Text;

namespace WlcDailyTrackAndroid
{
	[Activity (Label = "ScoreActivity", Theme = "@style/Theme.AppCompat.Light")]			
	public class ScoreActivity : BaseActivity, Android.Support.V7.App.ActionBar.ITabListener
	{
		private ViewPager sectionsViewPager;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var pagerAdapter = new SectionsPagerAdapter (this.SupportFragmentManager);
			SetContentView (Resource.Layout.activity_score);


			SupportActionBar.SetHomeButtonEnabled(false);
			SupportActionBar.SetDisplayShowTitleEnabled(false);
			SupportActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.Transparent));

			SupportActionBar.NavigationMode = (int) ActionBarNavigationMode.Tabs;
			SupportActionBar.SetStackedBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.Transparent));

			sectionsViewPager = FindViewById <ViewPager> (Resource.Id.sections_pager);
			sectionsViewPager.Adapter = pagerAdapter;
			sectionsViewPager.OffscreenPageLimit = (int)TrimMemory.RunningModerate;
			sectionsViewPager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) => {
				SupportActionBar.SetSelectedNavigationItem(e.Position);
			};

			// Add a tab to the action bar for each subsection
			for (int i = 0; i < pagerAdapter.Count; i++) {
				Android.Support.V7.App.ActionBar.Tab scheduleTab = SupportActionBar.NewTab ().SetIcon(pagerAdapter.PageIcon(i));				
				scheduleTab.SetTabListener (this);

				SupportActionBar.AddTab(scheduleTab);
			}
		}

		public void OnTabReselected (Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{

		}

		public void OnTabSelected (Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{
			sectionsViewPager.CurrentItem = tab.Position;
//			Drawable icon = actionBar.getSelectedTab().getIcon();
//			if (icon != null) {
//				icon.setAlpha(255);
//			}
		}

		public void OnTabUnselected (Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
		{

		}

		public class SectionsPagerAdapter : FragmentStatePagerAdapter {
			public SectionsPagerAdapter (SupportFragmentManager fm) : base (fm) {

			}

			public override Android.Support.V4.App.Fragment GetItem (int position)
			{
				SupportFragment fragment;
				Bundle args;
				switch (position) {
				case 0:
					fragment = new DashboardFragment ();
					break;
				case 1:
					fragment = new StatsFragment ();
					args = new Bundle ();
					args.PutInt (StatsFragment.ARG_STATS, position);
					fragment.Arguments = args;
					break;
				case 2:
					fragment = new LeaderboardFragment ();
					args = new Bundle ();
					args.PutInt (LeaderboardFragment.ARG_LEADERBOARD, position);
					fragment.Arguments = args;
					break;
				case 3:
					fragment = new WebViewFragment ();
					//https://www.wholelifechallenge.com/complete-game-rules-whole-life-challenge/
					args = new Bundle ();
					args.PutString (WebViewFragment.ARG_URL, "https://www.wholelifechallenge.com/complete-game-rules-whole-life-challenge/");
					args.PutInt (WebViewFragment.ARG_POSITION, position);
					fragment.Arguments = args;
					break;
				case 4:
					fragment = new WebViewFragment ();
					args = new Bundle ();
					args.PutString (WebViewFragment.ARG_URL, "https://www.wholelifechallenge.com/complete-game-rules-whole-life-challenge/");
					args.PutInt (WebViewFragment.ARG_POSITION, position);
					fragment.Arguments = args;
					break;
				default:
					fragment = new DashboardFragment ();
					break;
				}

				return fragment;
			}

			public override int Count {
				get {
					return 3;
				}
			}

			public int PageIcon (int position) {
				switch (position) {
				case 0:
					return Resource.Drawable.ic_action_home_256;
				case 1:
					return Resource.Drawable.ic_action_bar_chart_256;
				case 2:
					return Resource.Drawable.ic_action_trophy_256;
				default:
					return Resource.Drawable.ic_action_home_256;
				}
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted (int position)
			{
				switch (position) {
				case 0:
					return new SpannableString( "Dashboard");
				case 1:
					return new SpannableString( "My Stats");
				case 2:
					return new SpannableString( "Leaderboard");
				case 3:
					return new SpannableString( "Rules");
				case 4:
					return new SpannableString( "Blog");
				default:
					return new SpannableString( "");
				}
					
			}
		}
	}
}

