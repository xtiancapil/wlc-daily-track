using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace Core
{
	public class WlcWebService
	{
		private const string challengeUrl = "https://game.wholelifechallenge.com/wlcny15";
		const string apiUrl = "https://game.wholelifechallenge.com/api/frontend/";
		public WlcWebService ()
		{
		}

		async public Task<string> GetStats(CookieContainer StoredCookies, string StatsUrl) {
			string htmlString = "";

			var handler = new HttpClientHandler () {
				CookieContainer = StoredCookies
			};
			var client = new HttpClient (handler);

			try {

				if(!String.IsNullOrEmpty(StatsUrl)) {
					var statsResp = await client.GetAsync(challengeUrl + StatsUrl);
					htmlString = await statsResp.Content.ReadAsStringAsync();
				}
			} catch (Exception e) {
				//TODO: do some sort of logging mechanism
			}

			return htmlString;
		}

		async public Task<DailyRecord> GetRecord (CookieContainer StoredCookies, string recordUrl, string csrfToken) {
			if (StoredCookies == null) {
				return null;
			}
			if (!recordUrl.Equals ("today.json") && !recordUrl.Equals ("yesterday.json")) {
				return null;
			}

			if (String.IsNullOrEmpty (csrfToken)) {
				return null;
			}

			DailyRecord _record = null;


			var handler = new HttpClientHandler () {
				CookieContainer = StoredCookies
			};
			var client = new HttpClient (handler);
			client.DefaultRequestHeaders.Add ("Accept", "application/json, text/plain, */*");
			client.DefaultRequestHeaders.Add ("X-CSRF-TOKEN", csrfToken);
			client.DefaultRequestHeaders.Add ("Referer", "https://game.wholelifechallenge.com/wlcfall14/hub");
			try {

				var recordResp = await client.GetAsync(challengeUrl + "/daily_records/" + recordUrl);
				var contentStr = recordResp.Content.ReadAsStringAsync();

				var _settings = new JsonSerializerSettings() {
					NullValueHandling = NullValueHandling.Ignore
				};
				//_record = JsonConvert.DeserializeObject<DailyRecord> (contentStr.Result);
				_record = JsonConvert.DeserializeObject<DailyRecord> (contentStr.Result, _settings);
			} catch (Exception ex) {

			}

			return _record;

		}

		async public Task<DailyRecord> PostRecord (CookieContainer StoredCookies, DailyRecord recordJson, string recordUrl, string csrfToken) {
			if (StoredCookies == null) {
				return null;
			}

//			if (!recordUrl.Equals ("today.json") && !recordUrl.Equals ("yesterday.json")) {
//				return null;
//			}

			if (String.IsNullOrEmpty (csrfToken)) {
				return null;
			}

			DailyRecord _record = null;

			var handler= new HttpClientHandler () {
				CookieContainer = StoredCookies
			};
			var client = new HttpClient (handler);
			client.DefaultRequestHeaders.Add ("Accept", "application/json, text/plain, */*");
			client.DefaultRequestHeaders.Add ("X-CSRF-TOKEN", csrfToken);
			client.DefaultRequestHeaders.Add ("Referer", "https://game.wholelifechallenge.com/wlcfall14/hub");
			try {
				var strPayload = JsonConvert.SerializeObject(recordJson);
				var httpContent = new StringContent ( strPayload, System.Text.Encoding.UTF8, "application/json");
				var recordResp = await client.PostAsync(challengeUrl + "/daily_records.json", httpContent);
				var contentStr = recordResp.Content.ReadAsStringAsync();


				var _settings = new JsonSerializerSettings() {
					NullValueHandling = NullValueHandling.Ignore
				};
				//_record = JsonConvert.DeserializeObject<DailyRecord> (contentStr.Result);
				_record = JsonConvert.DeserializeObject<DailyRecord> (contentStr.Result, _settings);
			} catch (Exception ex) {

			}

			return _record;
		}

		async public Task<byte[]> GetProfileImage(string imageUrl) {
			byte[] imageBytes = null;

			var client = new HttpClient ();
			imageBytes = await client.GetByteArrayAsync(imageUrl);

			return imageBytes;
		}

		async public Task<Feed> GetReflections(int teamId, float cursor, int pageSize, CookieContainer storedCookies, string csrfToken) {
			if (storedCookies == null) {
				return null;
			}

			if (String.IsNullOrEmpty (csrfToken)) {
				return null;
			}

			Feed feed = null;

			var handler= new HttpClientHandler () {
				CookieContainer = storedCookies
			};
			var client = new HttpClient (handler);
			client.DefaultRequestHeaders.Add ("Accept", "application/json, text/plain, */*");
			client.DefaultRequestHeaders.Add ("Host", "game.wholelifechallenge.com");
			client.DefaultRequestHeaders.Add ("X-CSRF-TOKEN", csrfToken);
			client.DefaultRequestHeaders.Add ("Referer", "https://game.wholelifechallenge.com/wlcny15/hub");
			try {
			
				var requestUrl = string.Format("{0}current_user/teams/{1}/posts.json?per={2}", apiUrl, teamId, pageSize, cursor);
				var recordResp = await client.GetAsync(requestUrl);
				var contentStr = recordResp.Content.ReadAsStringAsync();


				var _settings = new JsonSerializerSettings() {
					NullValueHandling = NullValueHandling.Ignore
				};
				//_record = JsonConvert.DeserializeObject<DailyRecord> (contentStr.Result);
				feed = JsonConvert.DeserializeObject<Feed> (contentStr.Result, _settings);
				//https://game.wholelifechallenge.com/api/frontend/current_user/teams/7569/posts.json?cursor=2307331&per=10
			} catch (Exception ex) {

			}

			return feed;
		}
	}
}

