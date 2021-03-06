﻿
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
using System.Net;
using Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using RestSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CounterWeightsDroid
{
	public class ReflectionFeedFragment :  BaseFragment
	{
		ListView reflectionList;
		View loadingView;
		View headerView;
		ChallengeProfile profile;
		ReflectionAdapter adapter;
		Android.Support.V4.Widget.SwipeRefreshLayout refreshView;

		WlcWebService webService;
		int position;
		string csrfToken;				

		bool isLoading;
		bool hasMoreItems;


		public static ReflectionFeedFragment NewInstance (int position) {

			var f = new ReflectionFeedFragment ();
			var b = new Bundle ();
			b.PutInt("position", position);
			f.Arguments = b;
			return f;
		}

		RestClient client;
		RestRequest postsReq;
		JsonSerializerSettings serializerSettings;

		async public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			webService = new WlcWebService ();

			var prefs = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			var myStatsUrl = prefs.GetString ("statsUrl", "");
			var challengeProfile = prefs.GetString ("challengeProfile", "");
			csrfToken = prefs.GetString ("csrfToken", "");
			adapter = new ReflectionAdapter (Activity);
			try {
				profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ChallengeProfile> (challengeProfile, new Newtonsoft.Json.JsonSerializerSettings() {
					NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
				});

				client = new RestClient ("https://game.wholelifechallenge.com");
				client.CookieContainer = StoredCookies;
				client.FollowRedirects = true;
				//TODO: parametrize the team id!
				postsReq = new RestRequest("api/frontend/current_user/teams/7569/posts.json", Method.GET);
				postsReq.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
				postsReq.AddHeader("X-CSRF-TOKEN", csrfToken);
				postsReq.AddHeader("Referer", "https://game.wholelifechallenge.com/wlcmay15/hub");
				postsReq.AddParameter("per", 10);
				var resp = await client.ExecuteGetTaskAsync(postsReq);
				var content = resp.Content;
				serializerSettings = new JsonSerializerSettings() {
					NullValueHandling = NullValueHandling.Ignore,
					DateParseHandling = DateParseHandling.None
				};
				//_record = JsonConvert.DeserializeObject<DailyRecord> (contentStr.Result);

				int teamId = profile.teams.Select(x => x.id).First();

				adapter.ReflectionFeed  = JsonConvert.DeserializeObject<Feed> (content, serializerSettings);
//				adapter.ReflectionFeed = await webService.GetReflections(teamId, 0, 10, StoredCookies, csrfToken);
				if(!Activity.IsFinishing) {
					UpdateView();
				} 
			} catch (Exception ex) {

			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			var v = inflater.Inflate (Resource.Layout.fragment_overview, container, false);
			reflectionList = v.FindViewById<ListView> (Resource.Id.stats_list);
			loadingView = inflater.Inflate (Resource.Layout.loading_view, null, false);
			reflectionList.AddFooterView(loadingView);
			loadingView.Visibility = ViewStates.Gone;
			reflectionList.Adapter = adapter;
			refreshView = v.FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout> (Resource.Id.refreshView);

			refreshView.Refresh += async delegate {
				postsReq.Parameters.RemoveAll(x => x.Name == "cursor");
				var resp = await client.ExecuteGetTaskAsync (postsReq);
				var feed = JsonConvert.DeserializeObject<Feed> (resp.Content, serializerSettings);

				adapter.ReflectionFeed = feed;

//				var feed = await webService.GetReflections(teamId, 0, 10, StoredCookies, csrfToken);

				//((ReflectionAdapter)adapter). = myStats.OrderByDescending(x => x.StatDate).ToList();

				refreshView.Refreshing = false;
				UpdateView();
			};
			UpdateView ();

			return v;		
		}

		public override void OnResume ()
		{
			base.OnResume ();
			reflectionList.Scroll += HandleScroll;
		}

		public override void OnPause ()
		{
			base.OnPause ();
			reflectionList.Scroll -= HandleScroll;
		}
		int lastVisibleItem;
		void HandleScroll (object sender, AbsListView.ScrollEventArgs e)
		{
			lastVisibleItem = e.FirstVisibleItem + e.VisibleItemCount;

			if (!isLoading && adapter.ReflectionFeed.pagination != null && (adapter.ReflectionFeed.pagination.next_cursor > 0) && (lastVisibleItem == e.TotalItemCount)) {
				isLoading = true;
				loadingView.Visibility = ViewStates.Visible;
				Task.Run (async delegate {
					postsReq.Parameters.RemoveAll(x => x.Name == "cursor");
					postsReq.AddParameter ("cursor", adapter.ReflectionFeed.pagination.next_cursor);

					var resp = client.Execute (postsReq);
					var feed = JsonConvert.DeserializeObject<Feed> (resp.Content, serializerSettings);

					adapter.ReflectionFeed.data.AddRange (feed.data);
					adapter.ReflectionFeed.pagination = feed.pagination;
					isLoading = false;
					Activity.RunOnUiThread(() => {
							UpdateView();
							loadingView.Visibility = ViewStates.Gone;
					});
				});
				// if we're at the bottom of the list, check that there's a next cursor number
			}
		}

		public void UpdateView () {
			if (adapter != null) {
				adapter.NotifyDataSetChanged ();
			}
		}
	}
}

