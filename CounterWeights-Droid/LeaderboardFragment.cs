
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
using RestSharp;
using Core;
using Newtonsoft.Json;
//using XamSvg;
using Android.Support.V4.View;
using com.refractored;
using System.Threading.Tasks;

namespace CounterWeightsDroid
{
	public class LeaderboardFragment : BaseFragment, ViewPager.IOnPageChangeListener
	{
		ViewPager pager;
		PagerSlidingTabStrip tabs;
		MyPagerAdapter adapter;

		View loadingView;
		ListView currentList;
		LeaderboardAdapter currentAdapter;
		Android.Support.V4.Widget.SwipeRefreshLayout currentLayout;

		//TODO: use DI to only set this once
		RestClient client;
		RestRequest request;
		ChallengeProfile profile;
		List<ListView> leaderboardListViews;
		List<LeaderboardAdapter> leaderboardAdapters;
		List<Android.Support.V4.Widget.SwipeRefreshLayout> refreshViews;

		const string WorldLeaderboardResourceUrl = "wlcny15/leaderboards.json";
		const string TeamLeaderboardResourceUrl = "wlcny15/teams/{0}/leaderboards.json";

		public static LeaderboardFragment NewInstance (int position) {
			var f = new LeaderboardFragment ();
			var b = new Bundle ();
			b.PutInt("position", position);
			f.Arguments = b;
			return f;
		}

		public void OnPageScrollStateChanged (int state)
		{

		}

		public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
		{
		}

		public void OnPageSelected (int position)
		{
//			if (currentList != null) {
//				currentList.Scroll -= HandleScroll;
//			}
			SetCurrentItems (position);
//			currentList.Scroll += HandleScroll;

			if (currentAdapter.Leaderboard.leaderboards.Count > 0) {
				SetLeaderboardRourceUrl ();
			} else {
				Task.Run (async () => await GetLeaderboardFeed());
			}
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			try {
				profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ChallengeProfile>(ChallengeProfileStr, new Newtonsoft.Json.JsonSerializerSettings() {
					NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
				});

//				client = new RestClient ("https://game.wholelifechallenge.com");
//				client.CookieContainer = StoredCookies;
//				client.FollowRedirects = true;
//				request = new RestRequest("wlcny15/teams/7569/leaderboards.json", Method.GET);
//				request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
//				request.AddHeader("X-CSRF-TOKEN", CSRFToken);
//				request.AddHeader("Referer", "https://game.wholelifechallenge.com/wlcny15/hub");
//				request.AddParameter("challenge_profile_id", profile.id);
//				request.AddParameter("per", 50);
//
//				var resp = await client.ExecuteGetTaskAsync(request);
//				var content = resp.Content;
//				var serializerSettings = new JsonSerializerSettings() {
//					NullValueHandling = NullValueHandling.Ignore,
//					DateParseHandling = DateParseHandling.None
//				};
//				adapter.Leaderboard = JsonConvert.DeserializeObject<Core.Leaderboard.Leaderboard>(content, serializerSettings);
//
//				if(!Activity.IsFinishing) {
//					UpdateView();
//				} 
			} catch {

			}
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate (Resource.Layout.fragment_tabstrip, container, false);

			loadingView = inflater.Inflate (Resource.Layout.loading_view, null, false);

			refreshViews = new List<Android.Support.V4.Widget.SwipeRefreshLayout> ();
			leaderboardAdapters = new List<LeaderboardAdapter> ();
			leaderboardListViews = new List<ListView> ();

			if (profile.teams.Count > 0) {
				foreach (var team in profile.teams) {
					var leaderboardRefreshView = new Android.Support.V4.Widget.SwipeRefreshLayout(Activity);
					var leaderboardListView = new ListView(Activity);
					var leaderboardAdapter = new LeaderboardAdapter (Activity);
					leaderboardListView.Adapter = leaderboardAdapter;
					leaderboardRefreshView.AddView (leaderboardListView);
					TeamHolder th = new TeamHolder();
					th.id = team.id;
					th.name = team.name;
					leaderboardRefreshView.Tag = th;
					refreshViews.Add(leaderboardRefreshView);
					leaderboardListViews.Add (leaderboardListView);
					leaderboardAdapters.Add (leaderboardAdapter);
				}
			}

			var lrv = new Android.Support.V4.Widget.SwipeRefreshLayout(Activity);
			var llv = new ListView(Activity);
			var lda = new LeaderboardAdapter (Activity);
			llv.Adapter = lda;
			lrv.AddView (llv);
			TeamHolder wth = new TeamHolder();
			wth.id = -1;
			wth.name = "World";
			lrv.Tag = wth;
			refreshViews.Add(lrv);
			leaderboardListViews.Add (llv);
			leaderboardAdapters.Add (lda);

			SetCurrentItems (0);

			adapter = new MyPagerAdapter (refreshViews);

			pager = v.FindViewById<ViewPager> (Resource.Id.pager);
			tabs = v.FindViewById<PagerSlidingTabStrip> (Resource.Id.tabs);
			pager.Adapter = adapter;
			tabs.SetViewPager (pager);
			tabs.OnPageChangeListener = this;
//			refreshView = v.FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout> (Resource.Id.refreshView);
//
//			refreshView.Refresh += async delegate {
//				var resp = await client.ExecuteGetTaskAsync(request);
//				var content = resp.Content;
//				var serializerSettings = new JsonSerializerSettings() {
//					NullValueHandling = NullValueHandling.Ignore,
//					DateParseHandling = DateParseHandling.None
//				};
//				adapter.Leaderboard = JsonConvert.DeserializeObject<Core.Leaderboard.Leaderboard>(content, serializerSettings);
//
//				refreshView.Refreshing = false;
//				UpdateView();
//			};
//			UpdateView ();

			return v;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			Task.Run (async () => await GetLeaderboardFeed ());
		}

		async Task GetLeaderboardFeed () {
			client = new RestClient ("https://game.wholelifechallenge.com");
			client.CookieContainer = StoredCookies;
			client.FollowRedirects = true;
			request = new RestRequest(Method.GET);
			request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
			request.AddHeader("X-CSRF-TOKEN", CSRFToken);
			request.AddHeader("Referer", "https://game.wholelifechallenge.com/wlcny15/hub");
			request.AddParameter("challenge_profile_id", profile.id);
			request.AddParameter("per", 50);
			SetLeaderboardRourceUrl ();

			var resp = await client.ExecuteGetTaskAsync(request);
			var content = resp.Content;
			var serializerSettings = new JsonSerializerSettings() {
				NullValueHandling = NullValueHandling.Ignore,
				DateParseHandling = DateParseHandling.None
			};

			Activity.RunOnUiThread ( () => {
				currentAdapter.Leaderboard = JsonConvert.DeserializeObject<Core.Leaderboard.Leaderboard>(content, serializerSettings);
				UpdateView();
			});
		}

		async Task RefreshLeaderboard () {
			var resp = await client.ExecuteGetTaskAsync(request);
			var content = resp.Content;
			var serializerSettings = new JsonSerializerSettings() {
				NullValueHandling = NullValueHandling.Ignore,
				DateParseHandling = DateParseHandling.None
			};

			Activity.RunOnUiThread (() => {
				currentAdapter.Leaderboard = JsonConvert.DeserializeObject<Core.Leaderboard.Leaderboard> (content, serializerSettings);
				currentLayout.Refreshing = false;
				UpdateView ();
			});
		}

		void SetCurrentItems (int position) {

			if (currentLayout != null) {
				currentLayout.Refresh -= HandleRefresh;
			}

			currentLayout = refreshViews [position];
			currentList = leaderboardListViews [position];
			currentAdapter = leaderboardAdapters [position];

			currentLayout.Refresh += HandleRefresh;
		}

		void HandleRefresh (object sender, EventArgs e)
		{
			Task.Run (async () => await RefreshLeaderboard ());
		}

		void SetLeaderboardRourceUrl () {
			if (request != null) {
				var id = ((TeamHolder)currentLayout.Tag).id;

				//TODO: convert to enums; -1 = User, -2 = World
				if (id == -1) {
					request.Resource = WorldLeaderboardResourceUrl;
				} else {
					request.Resource = string.Format (TeamLeaderboardResourceUrl, id);
				}
			}
		}

		void UpdateView () {
			if (currentAdapter != null) {
				currentAdapter.NotifyDataSetChanged ();
			}
		}

		class TeamHolder : Java.Lang.Object {
			public int id { get; set; }
			public string name { get; set; }
		}

		class MyPagerAdapter : PagerAdapter {
			List<Android.Support.V4.Widget.SwipeRefreshLayout> items;

			public MyPagerAdapter (List<Android.Support.V4.Widget.SwipeRefreshLayout> items) {
				this.items = items;
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted (int position)
			{
				var txt = items [position].Tag as TeamHolder;
				return new Java.Lang.String(txt.name);
			}

			public override Java.Lang.Object InstantiateItem (ViewGroup container, int position)
			{						
				container.AddView (items [position]);
				return items [position];
			}

			public override void DestroyItem (ViewGroup container, int position, Java.Lang.Object @object)
			{
				container.RemoveView ((View)@object);
			}

			public override bool IsViewFromObject (View view, Java.Lang.Object @object)
			{
				return view == @object;
			}

			public override int Count {
				get {
					return items.Count;
				}
			}
		}
	}
}

