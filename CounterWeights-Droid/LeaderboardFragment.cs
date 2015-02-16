
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
using XamSvg;

namespace CounterWeightsDroid
{
	public class LeaderboardFragment : BaseFragment
	{
		//TODO: use DI to only set this once
		RestClient client;
		RestRequest request;
		ChallengeProfile profile;
		ListView leaderboardListView;
		LeaderboardAdapter adapter;
		Android.Support.V4.Widget.SwipeRefreshLayout refreshView;

		public static LeaderboardFragment NewInstance (int position) {
			var f = new LeaderboardFragment ();
			var b = new Bundle ();
			b.PutInt("position", position);
			f.Arguments = b;
			return f;
		}

		async public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			adapter = new LeaderboardAdapter (Activity);
			try {
				profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ChallengeProfile>(ChallengeProfileStr, new Newtonsoft.Json.JsonSerializerSettings() {
					NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
				});

				client = new RestClient ("https://game.wholelifechallenge.com");
				client.CookieContainer = StoredCookies;
				client.FollowRedirects = true;
				request = new RestRequest("wlcny15/teams/7569/leaderboards.json", Method.GET);
				request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
				request.AddHeader("X-CSRF-TOKEN", CSRFToken);
				request.AddHeader("Referer", "https://game.wholelifechallenge.com/wlcny15/hub");
				request.AddParameter("challenge_profile_id", profile.id);
				request.AddParameter("per", 50);

				var resp = await client.ExecuteGetTaskAsync(request);
				var content = resp.Content;
				var serializerSettings = new JsonSerializerSettings() {
					NullValueHandling = NullValueHandling.Ignore,
					DateParseHandling = DateParseHandling.None
				};
				adapter.Leaderboard = JsonConvert.DeserializeObject<Core.Leaderboard.Leaderboard>(content, serializerSettings);

				if(!Activity.IsFinishing) {
					UpdateView();
				} 
			} catch {

			}
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate (Resource.Layout.fragment_overview, container, false);
			var h = inflater.Inflate (Resource.Layout.list_header_leaderboard, null);
			leaderboardListView = v.FindViewById<ListView> (Resource.Id.stats_list);
			leaderboardListView.AddHeaderView (h);
			leaderboardListView.Adapter = adapter;

			var bmp = SvgFactory.GetBitmap (Activity.Resources, Resource.Raw.nutrition, 48, 48);
			h.FindViewById <ImageView> (Resource.Id.nutritionScore).SetImageBitmap (bmp);

			bmp = SvgFactory.GetBitmap (Activity.Resources, Resource.Raw.exercise, 48, 48);
			h.FindViewById <ImageView> (Resource.Id.exerciseScore).SetImageBitmap (bmp);

			bmp = SvgFactory.GetBitmap (Activity.Resources, Resource.Raw.stretching, 48, 48);
			h.FindViewById <ImageView> (Resource.Id.stretchingScore).SetImageBitmap (bmp);

			bmp = SvgFactory.GetBitmap (Activity.Resources, Resource.Raw.supplement, 48, 48);
			h.FindViewById <ImageView> (Resource.Id.supplementScore).SetImageBitmap (bmp);

			bmp = SvgFactory.GetBitmap (Activity.Resources, Resource.Raw.water, 48, 48);
			h.FindViewById <ImageView> (Resource.Id.waterScore).SetImageBitmap (bmp);

			bmp = SvgFactory.GetBitmap (Activity.Resources, Resource.Raw.lifestyle, 48, 48);
			h.FindViewById <ImageView> (Resource.Id.lifestyleScore).SetImageBitmap (bmp);

			bmp = SvgFactory.GetBitmap (Activity.Resources, Resource.Raw.reflection, 48, 48);
			h.FindViewById <ImageView> (Resource.Id.reflectionScore).SetImageBitmap (bmp);

			refreshView = v.FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout> (Resource.Id.refreshView);

			refreshView.Refresh += async delegate {
				var resp = await client.ExecuteGetTaskAsync(request);
				var content = resp.Content;
				var serializerSettings = new JsonSerializerSettings() {
					NullValueHandling = NullValueHandling.Ignore,
					DateParseHandling = DateParseHandling.None
				};
				adapter.Leaderboard = JsonConvert.DeserializeObject<Core.Leaderboard.Leaderboard>(content, serializerSettings);

				refreshView.Refreshing = false;
				UpdateView();
			};
			UpdateView ();

			return v;
		}

		void UpdateView () {
			if (adapter != null) {
				adapter.NotifyDataSetChanged ();
			}
		}
	}
}

