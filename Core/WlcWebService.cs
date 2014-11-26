using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace Core
{
	public class WlcWebService
	{
		private const string challengeUrl = "https://game.wholelifechallenge.com/wlcfall14";
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
	}
}

