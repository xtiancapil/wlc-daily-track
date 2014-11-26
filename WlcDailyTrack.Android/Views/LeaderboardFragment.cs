
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
using HtmlAgilityPack;
using Android.Graphics;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WlcDailyTrackAndroid
{
	public class LeaderboardFragment : Android.Support.V4.App.Fragment
	{
		public static string ARG_LEADERBOARD = "leaderboard";
		// Sample stats
		public static string stats_url = "https://game.wholelifechallenge.com/wlcsummer14/my_stats";
		public static string login_url = "https://game.wholelifechallenge.com/login";
		public static string hub_url = "https://game.wholelifechallenge.com/wlcsummer14/hub";

		private CookieContainer cookies;
		private HtmlDocument doc;

		private View headingView;
		private TextView headingText;
		private ProgressBar loading;
		private ListView listView;
		private LeaderboardListAdapter adapter;
		private List<Core.LeaderboardItem> rankings;

		private string leaderBoardUrl;
		private string userTeamsJson;

		public async override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var prefs = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			leaderBoardUrl = prefs.GetString ("leaderBoardUrl", "");
			userTeamsJson = prefs.GetString ("userTeams", "");
			adapter = new LeaderboardListAdapter (this.Activity);
			doc = new HtmlAgilityPack.HtmlDocument ();
			rankings = new List<Core.LeaderboardItem> ();
			var stringHtml = await GetLeaderboardHtml ();

			adapter.Leaderboard = rankings;

			if (!Activity.IsFinishing) {
				updateView ();
				if (loading != null) {
					loading.Visibility = ViewStates.Gone;
					listView.Visibility = ViewStates.Visible;
				}
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View v = inflater.Inflate (Resource.Layout.fragment_stats, container, false);

			v.FindViewById<TextView> (Resource.Id.greeting).Text = "Leaderboard";
			v.FindViewById<LinearLayout> (Resource.Id.welcome_color_block).SetBackgroundResource (Resource.Color.leaderboard);
			loading = v.FindViewById<ProgressBar> (Resource.Id.loadingBar);
			loading.Indeterminate = true;

			listView = v.FindViewById <ListView> (Resource.Id.welcome_details_list);
			listView.Adapter = adapter;

			updateView ();

			return v;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			updateView ();
		}

		private void updateView() {
			if (adapter != null) {
				adapter.NotifyDataSetChanged ();
			}
		}

		//TODO: move this to a common util class
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

						//						var serializer = new XmlSerializer (typeof(CookieContainer));
						//						cookies = (CookieContainer)serializer.Deserialize (new System.IO.StringReader (serialized));
					}
				}
				return cookies ?? (cookies = new CookieContainer ());
			}
		}

		async Task<string> GetLeaderboardHtml() {
			string htmlString = "";
			try {	
				var handler = new HttpClientHandler () {
					CookieContainer = StoredCookies
				};
				var client = new HttpClient (handler);

				// get the teams json
				var statsResp = await client.GetAsync("https://game.wholelifechallenge.com/wlcfall14" + leaderBoardUrl);
				htmlString = await statsResp.Content.ReadAsStringAsync();

				JObject jo = JObject.Parse(htmlString);
				var teams = jo["teams"].Select(t => t.ToObject<Core.Team>()).ToList();//.ToObject<Core.Team>();//.Select( t => (string) t).ToList();
				//var teams = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.Team>>(userTeamsJson);



				return "";

				doc.LoadHtml(htmlString);

				var teamNode = doc.GetElementById("my-team");
				var tableNode = teamNode.ChildNodes.FindFirst("table");
//				var tbody = tableNode.ChildNodes.FindFirst("tbody");
				var playerRows = tableNode.ChildNodes.Where(row => row.Name =="tr").ToArray();
			
				for(int i = 0; i < playerRows.Length; i++){

					if(!playerRows[i].HasChildNodes){ 
						continue;
					}

					var playerStats = playerRows[i].ChildNodes.Where(row => row.Name =="td").ToArray();
					var wlcPlayer = new Core.LeaderboardItem();
					for(int j = 0; j < playerStats.Length; j++) {
						int _val = 0;
						switch(j){
						case 0:
							int.TryParse(playerStats[j].InnerText, out _val);
							wlcPlayer.Rank = _val;
							break;
						case 1:
							wlcPlayer.Player = playerStats[j].InnerText.Replace("\n", "").Replace("\r", "");
							break;
						case 2:
							wlcPlayer.Teams = playerStats[j].InnerText.Replace("\n", "").Replace("\r", "");
							break;
						case 3:
							int.TryParse(playerStats[j].InnerText, out _val);
							wlcPlayer.Total = _val;
							break;
						}

					}

					rankings.Add(wlcPlayer);


				}


			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}

			return htmlString;
		}
	}
}

