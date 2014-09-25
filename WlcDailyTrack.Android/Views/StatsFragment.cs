
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
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WlcDailyTrackAndroid
{
	public class StatsFragment : Android.Support.V4.App.Fragment
	{
		public static string ARG_STATS = "stats";
		// Sample stats
		public static string stats_url = "https://game.wholelifechallenge.com/wlcsummer14/my_stats";
		public static string login_url = "https://game.wholelifechallenge.com/login";
		public static string hub_url = "https://game.wholelifechallenge.com/wlcsummer14/hub";

		private CookieContainer cookies;
		private HtmlDocument doc;
		private StatsListAdapter adapter;

		private List<Core.Stat> myStats;
		private ProgressBar loading;
		private ListView listView;

		private string myStatsUrl;

		public async override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var prefs = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			myStatsUrl = prefs.GetString ("statsUrl", "");

			doc = new HtmlAgilityPack.HtmlDocument ();
			adapter = new StatsListAdapter (this.Activity);
			myStats = new List<Core.Stat> ();
			string stringHtml = await GetStats();
			ProcessHtml (stringHtml);

			adapter.Stats = myStats.OrderByDescending(x => x.StatDate).ToList();

			if (!Activity.IsFinishing) {
				updateView ();
				if (loading != null) {
					loading.Visibility = ViewStates.Gone;
					listView.Visibility = ViewStates.Visible;
				}
			}
//			await GetStats ();
			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View v = inflater.Inflate (Resource.Layout.fragment_stats, container, false);

			v.FindViewById<TextView> (Resource.Id.greeting).Text = "My Stats";
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

		private void updateView () {

			if (adapter != null) {
				adapter.NotifyDataSetChanged ();
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

//						var serializer = new XmlSerializer (typeof(CookieContainer));
//						cookies = (CookieContainer)serializer.Deserialize (new System.IO.StringReader (serialized));
					}
				}
				return cookies ?? (cookies = new CookieContainer ());
			}
		}

		async Task<string> GetStats() {
			string htmlString = "";

			var handler = new HttpClientHandler () {
				CookieContainer = StoredCookies
			};
			var client = new HttpClient (handler);

			try {

				if(!String.IsNullOrEmpty(myStatsUrl)) {
					var statsResp = await client.GetAsync("https://game.wholelifechallenge.com/wlcsummer14" + myStatsUrl);
					htmlString = await statsResp.Content.ReadAsStringAsync();
				}
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}

			return htmlString;
		}


		void ProcessHtml(string statsString) {

			doc.LoadHtml (statsString);

//			var body = doc.DocumentNode.ChildNodes.FindFirst ("body");
			var div = doc.DocumentNode.ChildNodes.FindFirst ("div");
			var table = div.ChildNodes.FindFirst ("table");
			var thead = table.ChildNodes.FindFirst ("thead");
			var tbody = table.ChildNodes.FindFirst ("tbody");

			var days = thead.ChildNodes.FindFirst("tr").Elements("td").ToArray();
			var fields = tbody.ChildNodes.Where (row => row.Name == "tr").ToArray ();

			List<List<HtmlNode>> dataFields = new List<List<HtmlNode>> ();

			for (int k = 0; k < fields.Length; k++) {
				dataFields.Add (fields [k].Elements ("td").ToList ());
			}

			// Go through the day first
			for (int i = 0; i < days.Length; i++) {
				var dayStat = new Core.Stat ();
				dayStat.Day = i + 1;
				dayStat.StatDate = Convert.ToDateTime (days [i].Element ("span").GetAttributeValue ("title", ""));

				for (int j = 0; j < fields.Length; j++) {
					if (days.Length != dataFields [j].Count || fields[j].ChildNodes.Count == 0)
						continue;

					var keyElement = fields [j].ChildNodes.FindFirst ("th").Element ("div");
					if (keyElement == null)
						continue;

					string key = keyElement.InnerText.Replace("\n","").Replace("\r","");
					var valueElement = (dataFields [j]) [i].Element ("span");
					if (valueElement == null) {
						valueElement = (dataFields [j]) [i];
					}

					int value = 0;

					int.TryParse (valueElement.InnerText.Replace ("\r", "").Replace ("\n", ""), out value);

					dayStat.Points.Add (key, value);
				}

				myStats.Add (dayStat);
			}				

		}
	}
}

