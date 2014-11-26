
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
using Newtonsoft.Json.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Core;
using Android.Graphics.Drawables;
using Splat;
using Android.Views.Animations;
using System.Threading.Tasks;

namespace WlcDailyTrackAndroid
{
	public class DashboardFragment : Android.Support.V4.App.Fragment
	{
		private CookieContainer cookies;
		JObject challengeProfileJson;
		private string userTeamsJson;
		private string myStatsUrl;
		private ArrayAdapter adapter;
		private DailyRecord today;
		private DailyRecord yesterday;

		private List<Core.Stat> myStats;
		private ProgressBar loading;
		private ListView listView;
		private TextView totalTextView;
		private Button recordTodayBtn;
		private RelativeLayout floatingHeader;
		private View headerView;
		private LinearLayout tableHeader; // row of icons inside the list view's header
		private CircleImageView profileImageView;
		private IBitmap profileImage;

		private TinyIoC.TinyIoCContainer container;
		private Drawable actionBarBackgroundDrawable;

		private Animation fadeoutAnimation, fadeinAnimation;
		private WlcWebService webService;
		private string csrfToken;

		private ChallengeProfile profile;

		async public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			HasOptionsMenu = true;
			container = TinyIoC.TinyIoCContainer.Current;
			//TODO: Add TinyIOC and move these to app start
			webService = container.Resolve<WlcWebService> ();
			var parser = new WlcHtmlParse ();

			var prefs = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			myStatsUrl = prefs.GetString ("statsUrl", "");
			csrfToken = prefs.GetString ("csrfToken", "");
			var challengeProfile = prefs.GetString ("challengeProfile", "");
			adapter = new StatsBarAdapter (this.Activity);

			fadeinAnimation = AnimationUtils.LoadAnimation (this.Activity, Resource.Animation.abc_fade_in);
			fadeoutAnimation = AnimationUtils.LoadAnimation (this.Activity, Resource.Animation.abc_fade_out);

			// Get the data here
			try {


				profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ChallengeProfile> (challengeProfile, new Newtonsoft.Json.JsonSerializerSettings() {
					NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
				});
				today = await webService.GetRecord(StoredCookies, "today.json", csrfToken);
				yesterday = await webService.GetRecord(StoredCookies, "yesterday.json", csrfToken);
				var contentStr = await webService.GetStats(StoredCookies, "/profiles/"+ profile.id.ToString() + "/stats_calendar");
				myStats = parser.GetStats(contentStr);

				if (today != null) {
					byte[] imageBytes = await webService.GetProfileImage(profile.user.photo);

					// IBitmap is a type that provides basic image information such as dimensions
					profileImage = await BitmapLoader.Current.Load(new MemoryStream(imageBytes), null /* Use original width */, null /* Use original height */);
				}

				((StatsBarAdapter)adapter).Stats = myStats.OrderByDescending(x => x.StatDate).ToList();
				if (!Activity.IsFinishing) {
					updateView ();
					if (loading != null) {
						loading.Visibility = ViewStates.Gone;
						listView.Visibility = ViewStates.Visible;
					}
				}

			

			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}				

			actionBarBackgroundDrawable = new Android.Graphics.Drawables.ColorDrawable (Android.Graphics.Color.MediumAquamarine);
			actionBarBackgroundDrawable.SetAlpha (0);
			Activity.ActionBar.SetBackgroundDrawable (actionBarBackgroundDrawable);

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View v = inflater.Inflate (Resource.Layout.fragment_dashboard, container, false);
			headerView = inflater.Inflate (Resource.Layout.list_header_stat, null, false);
			floatingHeader = v.FindViewById <RelativeLayout> (Resource.Id.floating_tableHeader);
			listView = v.FindViewById <ListView> (Resource.Id.stats_list);
			loading = v.FindViewById <ProgressBar> (Resource.Id.loadingBar);

			listView.AddHeaderView (headerView);
			listView.Adapter = adapter;
			totalTextView = headerView.FindViewById <TextView> (Resource.Id.totalTextView);
			recordTodayBtn = headerView.FindViewById <Button> (Resource.Id.recordTodayBtn);
			tableHeader = headerView.FindViewById <LinearLayout> (Resource.Id.header);
			profileImageView = headerView.FindViewById <CircleImageView> (Resource.Id.profileImageView);

			var prefs = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			var challengeProfileStr = prefs.GetString ("challengeProfile", "");
			userTeamsJson = prefs.GetString ("userTeams", "");
//			try {
//
//				JObject challengeProfileJson = JObject.Parse (challengeProfileStr);
//				var userName = (string)challengeProfileJson ["user"] ["full_name"];
//				headerView.FindViewById<TextView> (Resource.Id.textView1).Text = userName;
//				headerView.FindViewById<TextView> (Resource.Id.textView1).Text = today.challenge_profile.user.full_name;
//			} catch (Exception ex) {
//				Console.WriteLine (ex);
//			}				

			updateView ();
			return v;
		}

		public override void OnResume ()
		{
			base.OnResume ();

			if (listView != null) {
				listView.Scroll += HandleScroll;
			}
		
		}

		public override void OnPause ()
		{
			base.OnPause ();

			if (listView != null) {
				listView.Scroll -= HandleScroll;
			}
		
		}



		public override bool OnOptionsItemSelected (IMenuItem item)
		{
//			switch(item.ItemId) {
//			case Resource.Id.action_enterScore:
//
//				today.reflection.content = "One of the few perfect days I've had! Worked out, ate right, smiled at people.";
//
//				var tsk = Task.Run (async () => await webService.PostRecord (StoredCookies, today, "daily_records.json", csrfToken));
//				var dr = tsk.Result;
//				return true;
//			}
			return base.OnOptionsItemSelected (item);
		}

		void HandleScroll (object sender, AbsListView.ScrollEventArgs e)
		{
			var h = headerView.Height - Activity.ActionBar.Height - floatingHeader.Height;
//			if (headerView != null) {
//				if ((tableHeader.Top + headerView.Top) < 110) {
//					floatingHeader.Visibility = ViewStates.Visible;
//				} else {
//					floatingHeader.Visibility = ViewStates.Gone;
//				}
//			}

			float ratio = (float) Math.Min(Math.Max(headerView.GetY () * -1, 0), h) / h;
			int newAlpha = (int) (ratio * 255);
			if (actionBarBackgroundDrawable != null) {
				actionBarBackgroundDrawable.SetAlpha (newAlpha);
//				Activity.ActionBar.SetBackgroundDrawable (actionBarBackgroundDrawable);
			}
		}

		private class ViewHolder {
			public TextView welcomeTitle { get; set; }
			public TextView welcomeContent { get; set; }
		}

		private void updateView () {

			if (adapter != null) {
				adapter.NotifyDataSetChanged ();
			}

			if (headerView != null && today != null && profile != null) {
				headerView.FindViewById<TextView> (Resource.Id.textView1).Text = profile.user.full_name;
			}

			if (totalTextView != null && today != null) {

				totalTextView.StartAnimation (fadeoutAnimation);
				totalTextView.Text = today.challenge_profile.total_score.ToString ();
				totalTextView.StartAnimation (fadeinAnimation);
			}				

			if (profileImageView != null && profileImage != null) {

				profileImageView.StartAnimation (fadeoutAnimation);
				profileImageView.SetImageDrawable ((BitmapDrawable) profileImage.ToNative());
				profileImageView.StartAnimation (fadeinAnimation);
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

