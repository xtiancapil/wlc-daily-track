
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
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using RestSharp;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace WlcDailyTrackAndroid
{
	[Activity (Label = "SignInActivity", Theme = "@style/WlcTheme.login")]			
	public class SignInActivity : Activity
	{
		private CookieContainer cookies;
		private EditText emailField, passwordField;

		private string csrfToken = "";
		private string csrfParam = "";

		//TODO: move this to config object
		public static string login_url = "https://game.wholelifechallenge.com/login";
		private HtmlDocument doc;
		private ProgressDialog pd;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			doc = new HtmlDocument ();
			// Create your application here
			SetContentView (Resource.Layout.activity_login);


			var button = FindViewById <Button> (Resource.Id.loginButton);
			button.Click += async delegate {
				await LoginButton_Click();
			};

			emailField = FindViewById <EditText> (Resource.Id.emailField);
			passwordField = FindViewById <EditText> (Resource.Id.passwordField);
			emailField.Text = "christian.capil@gmail.com";
			passwordField.Text = "lakers";

			TextView tos = FindViewById <TextView> (Resource.Id.tos);
			tos.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;
		}

		protected override void OnPause ()
		{
			base.OnPause ();
		}

		private void StoreCookie () {
			var binaryFormatter = new BinaryFormatter ();
			var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
			var editor = prefs.Edit ();
			if (cookies != null && cookies.Count > 0) {
				var buff = new MemoryStream ();
				binaryFormatter.Serialize (buff, cookies);
				var buffString = Convert.ToBase64String(buff.GetBuffer());
				editor.PutString ("cookies", buffString);
			}

			editor.Commit ();		
		}

		async Task LoginButton_Click () {
			try {
				var username = emailField.Text;
				var pass = passwordField.Text;

				pd = new ProgressDialog(this);
				pd.Indeterminate = true;
				pd.SetCancelable (false);
				pd.SetMessage ("Logging in...");

				RunOnUiThread( () => {
					pd.Show();
				});
				var loginStatus = await Login(username, pass);

				RunOnUiThread( () => {
					pd.Dismiss();
				});
				 
				if(String.IsNullOrEmpty(loginStatus)){
					FinishActivity();
					return;
				}

				emailField.Error = loginStatus;
//
//				await Parse.ParseUser.LogInAsync (username, pass);
//				Console.WriteLine (Parse.ParseUser.CurrentUser.Username);
//				FinishActivity();
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
		}

		private void FinishActivity() {
			// Start an intent for the dispatch activity
			Intent intent = new Intent(this, typeof(DispatchActivity));
			intent.AddFlags (ActivityFlags.ClearTask | ActivityFlags.NewTask);
			StartActivity(intent);
		}

		CookieContainer StoredCookies {
			get {
				if (cookies == null) {
					var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
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

		Core.WlcCredentials StoredCredentials {
			get {
				var prefs = this.GetSharedPreferences("wlcPrefs", FileCreationMode.Private);
				return new Core.WlcCredentials {
					Username = prefs.GetString ("wlcUsername", null),
					Password = prefs.GetString ("wlcPass", null),
				};
			}
			set {
				var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
				var editor = prefs.Edit ();
				editor.PutString ("wlcUsername", value.Username ?? string.Empty);
				editor.PutString ("wlcPass", value.Password ?? string.Empty);
				editor.Commit ();
			}
		}

		// Just login and
		async Task<string> Login(string email, string password) {
			string htmlString = "";

			try {
				var client = new RestClient ("https://game.wholelifechallenge.com");
				client.CookieContainer = StoredCookies;
				client.FollowRedirects = false;
				var loginFormReq = new RestRequest ("login", Method.GET);

				loginFormReq.AddHeader("Accept", "text/html");
				var loginFormResp = await client.ExecuteTaskAsync(loginFormReq); //(RestResponse) client.Execute(loginFormReq); //await client.ExecuteTaskAsync(loginFormReq);
				var loginHtml = ((RestResponse) loginFormResp).Content;

				doc.LoadHtml (loginHtml);
				var head = doc.DocumentNode.ChildNodes.FindFirst ("head");
				var metas = head.ChildNodes.Where (x => x.Name == "meta").ToList ();

				foreach (var meta in metas) {
					if (meta.GetAttributeValue ("name", "").Equals ("csrf-param", StringComparison.OrdinalIgnoreCase)) {
						csrfParam = meta.GetAttributeValue ("content", "");
					}

					if (meta.GetAttributeValue ("name", "").Equals ("csrf-token", StringComparison.OrdinalIgnoreCase)) {
						csrfToken = meta.GetAttributeValue ("content", "");
					}
				}

				var loginPostReq = new RestRequest("login", Method.POST);
				loginPostReq.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

				var content = new FormUrlEncodedContent(new Dictionary<string, string> (){
					{"utf8", "✓"},
					{csrfParam, csrfToken},
					{"user[email]", email},
					{"user[password]", password},
					{"user[remember_me]", "1"},
					{"commit", "Sign In"}
				});
					
				loginPostReq.AddHeader("Referer", "https://game.wholelifechallenge.com/login");
				loginPostReq.AddParameter("utf8",  "✓");
				loginPostReq.AddParameter(csrfParam, csrfToken);
				loginPostReq.AddParameter("user[email]", email);
				loginPostReq.AddParameter("user[password]", password);
				loginPostReq.AddParameter("user[remember_me]", 1);
				loginPostReq.AddParameter("commit", "Sign In");

				var resp = client.Execute(loginPostReq);
				htmlString = resp.Content;

				// If the status code is 302, it means we have logged in correctly and are being
				// redirected to the hub page. This will probably break though :\
				if(resp.StatusCode == HttpStatusCode.Redirect) {
					// We logged in correctly, store the credentials and session cookies before doing anything else.
					StoredCredentials = new Core.WlcCredentials() {
						Username = email,
						Password = password
					};
					StoreCookie();

					// Get the initial hub page to get the stats , leaderboard, and reflection urls
					var hubReq = new RestRequest("wlcfall14/hub", Method.GET);
					hubReq.AddHeader("Accept", "text/html");

					var hubResp = (RestResponse) client.Execute(hubReq);
					var hubHtml = hubResp.Content;

					//HACK: Get the IDs instead of the route for the stats and team id.
					var profileMatch = Regex.Match(hubHtml, "/profiles/[0-9]+/stats_calendar");
					var leaderboardMatch = Regex.Match(hubHtml, "/challenge_profiles/[0-9]+/teams.json");
					var userTeams = Regex.Match(hubHtml, "'userTeams', (\\[(.*?)\\])");
					var challengeProfile = Regex.Match(hubHtml, "\\('challengeProfile',[\\s]+({(.?)+\\})");
					//var challengeProfile = Regex.Match(hubHtml, "\\('challengeProfile', \\{[\\s]\\}\\)\s+</script>");
//					challengeProfile = Regex.Match(hubHtml, "'challengeProfile', function\\(\\)\\{[\\s]+return (.*?)[\\s]+\\}");

					var prefs = this.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);
					var editor = prefs.Edit ();
					editor.PutString("statsUrl", profileMatch.Success ? profileMatch.Value : String.Empty);
					editor.PutString("leaderBoardUrl", leaderboardMatch.Success ? leaderboardMatch.Value : String.Empty);
					editor.PutString("userTeams", userTeams.Success ? userTeams.Groups[1].Value : String.Empty);
					editor.PutString("challengeProfile", challengeProfile.Success ? challengeProfile.Groups[1].Value : String.Empty);
					editor.PutString("csrfToken", csrfToken);
					editor.Commit();

					htmlString = "";
				}

				// Get the error text and surface it to the screen.
				var match = Regex.Match(htmlString, "flash_alert");
				if(match.Success) {						
					doc.LoadHtml(htmlString);
					var _alert = doc.GetElementById("flash_alert");
					htmlString = _alert.InnerText;
				}

//				var loginReq = new HttpRequestMessage(HttpMethod.Post, login_url);
//				loginReq.Content = content;
//
//				var loginResp = await client.SendAsync(loginReq).ConfigureAwait(false);
//				// this is where all the content is stored. Need to move this to the login section.
//				htmlString = await loginResp.Content.ReadAsStringAsync().ConfigureAwait(false);
//
//				// If the status code is 302, it means we have logged in correctly and are being
//				// redirected to the hub page. This will probably break though :\
//				if(loginResp.StatusCode == HttpStatusCode.Redirect) {
//					StoredCredentials = new Core.WlcCredentials() {
//						Username = email,
//						Password = password
//					};
//					FinishActivity();
//					return;
//				}
//
//				// Get the error code and surface it to the screen.
//				var match = Regex.Match(htmlString, "flash_alert");
//				if(match.Success) {					
//					doc.LoadHtml(htmlString);
//					var _alert = doc.GetElementById("flash_alert");
//					Console.WriteLine(_alert.InnerText);
//				}

//				var hubResp = await client.GetAsync("https://game.wholelifechallenge.com/wlcsummer14/hub");
//				var hubHtml = await hubResp.Content.ReadAsStringAsync();
//				var regex = new Regex("/profiles/[0-9]+/stats_calendar");
//				var match = regex.Match(hubHtml);
//
//				// Go to the actual hub we want
//
//				var leaderboardMatch = Regex.Match(hubHtml, "team=[0-9]+");
//
//				var statsResp = await client.GetAsync("https://game.wholelifechallenge.com/wlcsummer14" + match.Value);
//				htmlString = await statsResp.Content.ReadAsStringAsync();
//						var cookie = cookieContainer.GetCookieHeader (new Uri ("https://game.wholelifechallenge.com"));

			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}	
			return htmlString;
		}
	}
}

