
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
using Android.Support.V4.View;
using com.refractored;
using Core;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace CounterWeightsDroid
{
	public class DailyPlayFragment : BaseFragment, ViewPager.IOnPageChangeListener
	{
		ViewPager pager;
		PagerSlidingTabStrip tabs;
		MyPagerAdapter adapter;

		View loadingView;
		ListView reflectionList, progressList, currentList;
		ReflectionAdapter reflectionAdapter, currentAdapter;
		StatsColorBarAdapter progressAdapter;
		Android.Support.V4.Widget.SwipeRefreshLayout reflectionRefresh, progressRefresh, currentLayout;

		RestClient client;
		RestRequest postsReq;
		ChallengeProfile profile;
		List<Android.Support.V4.Widget.SwipeRefreshLayout> reflections;
		List<ListView> reflectionsList;
		List<ReflectionAdapter> reflectionsAdapter;
		bool isLoading, hasMoreItems;
		int lastVisibleItem;

		const string TeamResourceUrl = "api/frontend/current_user/teams/{0}/posts.json";
		const string UserResourceUrl = "api/frontend/current_user/posts.json";
		const string WorldResourceUrl = "api/frontend/challenges/8/posts.json";

		public static DailyPlayFragment NewInstance (int position) {
			var f = new DailyPlayFragment ();
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
			if (currentList != null) {
				currentList.Scroll -= HandleScroll;
			}
			currentLayout = reflections [position];
			currentList = reflectionsList [position];
			currentAdapter = reflectionsAdapter [position];
			currentList.Scroll += HandleScroll;

			if (currentAdapter.ReflectionFeed.data.Count > 0) {
				SetFeedResourceUrl ();
			} else {
				Task.Run (async () => await GetInitialFeed());
			}
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			reflectionList = new ListView (this.Activity);
			reflectionList.SetBackgroundColor (Android.Graphics.Color.Fuchsia);
			progressList = new ListView (this.Activity);

			reflectionRefresh = new Android.Support.V4.Widget.SwipeRefreshLayout (this.Activity);
			reflectionRefresh.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			reflectionRefresh.AddView (reflectionList);

			progressRefresh = new Android.Support.V4.Widget.SwipeRefreshLayout (this.Activity);
			progressRefresh.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			progressRefresh.AddView (progressList);

		//	adapter = new MyPagerAdapter (new [] {reflectionRefresh, progressRefresh});
			reflectionAdapter = new ReflectionAdapter (Activity);
			progressAdapter = new StatsColorBarAdapter (Activity);
//
			try {

				profile = JsonConvert.DeserializeObject<ChallengeProfile> (ChallengeProfileStr, new JsonSerializerSettings() {
					NullValueHandling = NullValueHandling.Ignore
				});

			} catch (Exception ex) {
			}
//
		}

		class TeamHolder : Java.Lang.Object {
			public int id { get; set; }
			public string name { get; set; }
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate (Resource.Layout.fragment_tabstrip, container, false);
			loadingView = inflater.Inflate (Resource.Layout.loading_view, null, false);
			reflections = new List<Android.Support.V4.Widget.SwipeRefreshLayout>();
			reflectionsList = new List<ListView> ();
			reflectionsAdapter = new List<ReflectionAdapter> ();
			if(profile.teams.Count > 0) {
				foreach(var team in profile.teams) {
					var rv = new Android.Support.V4.Widget.SwipeRefreshLayout(Activity);
					var lv = new ListView(Activity);
					var adp = new ReflectionAdapter (Activity);
					lv.Adapter = adp;
					rv.AddView (lv);
					TeamHolder th = new TeamHolder();
					th.id = team.id;
					th.name = team.name;
					Console.WriteLine(team.slug);
					rv.Tag = th;
					reflections.Add(rv);
					reflectionsList.Add (lv);
					reflectionsAdapter.Add (adp);
				}
			}

			var uv = new Android.Support.V4.Widget.SwipeRefreshLayout(Activity);
			var ulv = new ListView(Activity);
			var ula = new ReflectionAdapter (Activity);
			ulv.Adapter = ula;
			uv.AddView (ulv);
			TeamHolder userTeam = new TeamHolder();
			userTeam.id = -1;
			userTeam.name = "Me";
			uv.Tag = userTeam;
			reflections.Add(uv);
			reflectionsList.Add (ulv);
			reflectionsAdapter.Add (ula);

			var wv = new Android.Support.V4.Widget.SwipeRefreshLayout(Activity);
			var wlv = new ListView(Activity);
			var wla = new ReflectionAdapter (Activity);
			wlv.Adapter = wla;
			wv.AddView (wlv);
			TeamHolder worldTeam = new TeamHolder();
			worldTeam.id = -2;
			worldTeam.name = "World";
			wv.Tag = worldTeam;
			reflections.Add(wv);
			reflectionsList.Add (wlv);
			reflectionsAdapter.Add (wla);


			currentLayout = reflections [0];
			currentList = reflectionsList [0];
			currentAdapter = reflectionsAdapter [0];
			currentList.Scroll += HandleScroll;
			adapter = new MyPagerAdapter(reflections);

			pager = v.FindViewById<ViewPager> (Resource.Id.pager);

			tabs = v.FindViewById<PagerSlidingTabStrip> (Resource.Id.tabs);
			pager.Adapter = adapter;
			tabs.SetViewPager (pager);
			tabs.OnPageChangeListener = this;


			return v;	
		}
		JsonSerializerSettings serializerSettings;
		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			Task.Run (async () => await GetInitialFeed());
		}

		async Task GetInitialFeed () {
			client = new RestClient ("https://game.wholelifechallenge.com");
			client.CookieContainer = StoredCookies;
			client.FollowRedirects = true;


			postsReq = new RestRequest(Method.GET);
			postsReq.AddHeader ("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
			postsReq.AddHeader ("X-CSRF-TOKEN", CSRFToken);
			postsReq.AddHeader ("Referer", "https://game.wholelifechallenge.com/wlcny15/hub");
			postsReq.AddParameter ("per", 10);
			SetFeedResourceUrl ();

			var resp = await client.ExecuteGetTaskAsync (postsReq);
			var content = resp.Content;
			serializerSettings = new JsonSerializerSettings () {
				NullValueHandling = NullValueHandling.Ignore,
				DateParseHandling = DateParseHandling.None
			};

			currentAdapter.ReflectionFeed = JsonConvert.DeserializeObject<Feed> (content, serializerSettings);
			Activity.RunOnUiThread(UpdateView);
		}

		void SetFeedResourceUrl () {

			if (postsReq != null) {
				var id = ((TeamHolder)currentLayout.Tag).id;

				//TODO: convert to enums; -1 = User, -2 = World
				if (id == -1) {
					postsReq.Resource = UserResourceUrl;
				} else if (id == -2) {
					postsReq.Resource = string.Format (WorldResourceUrl, profile.id);
				} else {
					postsReq.Resource = string.Format (TeamResourceUrl, id);
				}
			}
		}

		void UpdateView () {

			if (currentAdapter != null) {
				currentAdapter.NotifyDataSetChanged ();
			}

			if (reflectionAdapter != null) {
				reflectionAdapter.NotifyDataSetChanged ();
			}

			if (progressAdapter != null) {
				progressAdapter.NotifyDataSetChanged ();
			}
		}

		void HandleScroll (object sender, AbsListView.ScrollEventArgs e)
		{
			lastVisibleItem = e.FirstVisibleItem + e.VisibleItemCount;

			if (!isLoading && currentAdapter.ReflectionFeed.pagination != null && (currentAdapter.ReflectionFeed.pagination.next_cursor > 0) && (lastVisibleItem == e.TotalItemCount)) {
				isLoading = true;
				currentList.AddFooterView (loadingView);
//				loadingView.Visibility = ViewStates.Visible;
				Task.Run (async delegate {
					postsReq.Parameters.RemoveAll(x => x.Name == "cursor");
					postsReq.AddParameter ("cursor", currentAdapter.ReflectionFeed.pagination.next_cursor);

					var resp = client.Execute (postsReq);
					var feed = JsonConvert.DeserializeObject<Feed> (resp.Content, serializerSettings);

					currentAdapter.ReflectionFeed.data.AddRange (feed.data);
					currentAdapter.ReflectionFeed.pagination = feed.pagination;
					isLoading = false;
					Activity.RunOnUiThread(() => {
						UpdateView();
						currentList.RemoveFooterView(loadingView);
						//loadingView.Visibility = ViewStates.Gone;
					});
				});
				// if we're at the bottom of the list, check that there's a next cursor number
			}
		}

		class MyPagerAdapter : PagerAdapter {
			string[] Titles = { "Reflections", "My Progress" };

			public ReflectionAdapter[] adapters { get; set; }
			List<Android.Support.V4.Widget.SwipeRefreshLayout> items;

			#region implemented abstract members of PagerAdapter

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

			#endregion


		}
	}
}

