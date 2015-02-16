
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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Core;

namespace CounterWeightsDroid
{
	public class BaseFragment : Android.Support.V4.App.Fragment
	{
		CookieContainer cookies;
		WlcWebService webService;
		ISharedPreferences preferences;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			preferences = Activity.GetSharedPreferences ("wlcPrefs", FileCreationMode.Private);

		}
			
		public CookieContainer StoredCookies {
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

		public WlcWebService WebService {
			get {
				return webService ?? (webService = new WlcWebService());
			}
		}

		public string StatsUrl {
			get {
				return preferences.GetString ("statusUrl", "");
			}
		}

		public string ChallengeProfileStr {
			get {
				return preferences.GetString ("challengeProfile", "");
			}
		}

		public string CSRFToken {
			get {
				return preferences.GetString ("csrfToken", "");
			}
		}

	}
}

