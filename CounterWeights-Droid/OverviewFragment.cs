
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
using Core;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading.Tasks;

namespace CounterWeightsDroid
{
	public class OverviewFragment : Android.Support.V4.App.Fragment
	{
		CookieContainer cookies;
		ListView statsList;
		View headerView;
		TextView totalScoreView;
		ChallengeProfile profile;
		ArrayAdapter adapter;

		WlcWebService webService;
		List<Core.Stat> myStats;
		DailyRecord today;
		RadialProgress.RadialProgressView averageScoreView;
		Android.Support.V4.Widget.SwipeRefreshLayout refreshView;
		string csrfToken;

		int position;
		public static OverviewFragment NewInstance(int position) {
			var f = new OverviewFragment ();
			var b = new Bundle ();
			b.PutInt("position", position);
			f.Arguments = b;
			return f;
		}

		async public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			webService = new WlcWebService ();

			var prefs = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			var myStatsUrl = prefs.GetString ("statsUrl", "");
			var challengeProfile = prefs.GetString ("challengeProfile", "");
			csrfToken = prefs.GetString ("csrfToken", "");
			adapter = new StatsColorBarAdapter (Activity);

			try {
				profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ChallengeProfile> (challengeProfile, new Newtonsoft.Json.JsonSerializerSettings() {
					NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
				});

				var contentStr = await webService.GetStats(StoredCookies, "/profiles/"+ profile.id.ToString() + "/stats_calendar");
				myStats = WlcHelpers.GetStats(contentStr);
				((StatsColorBarAdapter)adapter).Stats = myStats.OrderByDescending(x => x.StatDate).ToList();
//				today = await webService.GetRecord(StoredCookies, "today.json", csrfToken); 
				if(!Activity.IsFinishing) {
					UpdateView();
				}
			} catch (Exception ex) {

			}

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate (Resource.Layout.fragment_overview, container, false);
			headerView = inflater.Inflate (Resource.Layout.list_header, null, false);
			statsList = v.FindViewById<ListView> (Resource.Id.stats_list);
//			statsList.AddHeaderView (headerView);
			statsList.Adapter = adapter;

			refreshView = v.FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout> (Resource.Id.refreshView);
			averageScoreView = headerView.FindViewById<RadialProgress.RadialProgressView> (Resource.Id.progressView);
			totalScoreView = headerView.FindViewById<TextView> (Resource.Id.totalScoreView);
			refreshView.Refresh += async delegate {
				var contentStr = await webService.GetStats(StoredCookies, "/profiles/"+ profile.id.ToString() + "/stats_calendar");
				myStats = WlcHelpers.GetStats(contentStr);
				((StatsColorBarAdapter)adapter).Stats = myStats.OrderByDescending(x => x.StatDate).ToList();
				refreshView.Refreshing = false;
				UpdateView();
			};
			UpdateView ();

			return v;
		}

		public void UpdateView() {

			if (adapter != null) {
				adapter.NotifyDataSetChanged ();
			}

			if (today != null) {
				averageScoreView.Value = (float) today.challenge_profile.average_score;
				totalScoreView.Text = today.challenge_profile.total_score.ToString ();
			}
		}
			
		CookieContainer StoredCookies {
			get {
				if (cookies == null) {
					var prefs = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
					var serialized = prefs.GetString ("cookies", null);
					if (serialized != null) {

						byte[] binData = Convert.FromBase64String(serialized);
						BinaryFormatter formatter = new BinaryFormatter();
						MemoryStream ms = new MemoryStream(binData);
						cookies = (CookieContainer) formatter.Deserialize(ms);
					}
				}
				return cookies ?? (cookies = new CookieContainer ());
			}
		}
	}
}

