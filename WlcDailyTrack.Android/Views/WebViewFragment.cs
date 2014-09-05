
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
using Android.Webkit;
using Android.Graphics.Drawables;
using Android.Graphics;
using ModernHttpClient;
using System.Net.Http;
using HtmlAgilityPack;

namespace WlcDailyTrackAndroid
{
	public class WebViewFragment : Android.Support.V4.App.Fragment
	{
		public static string ARG_URL = "url";
		public static string ARG_POSITION = "position";
		string url, html;
		int position;
		WebView webView;
		bool isRulesPage;
		public async override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			url = Arguments.GetString (ARG_URL, "");
			position = Arguments.GetInt (ARG_POSITION, -1);
			isRulesPage = position == 3;
			var httpClient = new HttpClient(new NativeMessageHandler());
			html = await httpClient.GetStringAsync (url);

			if (webView != null) {
				string content = "";


				var doc = new HtmlDocument ();
				doc.LoadHtml (html);
				if (isRulesPage) {
					var node = doc.GetElementById (GetString (Resource.String.rules_id));
					if (node != null) {
						content = node.WriteTo ();
					}
				} else {
					content = "<h1>Blog</h1>";
				}

				webView.LoadData (content, "text/html", "UTF-8");
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View v = inflater.Inflate (Resource.Layout.fragment_webview, container, false);

			var colorBlock = v.FindViewById <LinearLayout> (Resource.Id.welcome_color_block);
			var welcomeText = v.FindViewById <TextView> (Resource.Id.greeting);

			if (position > -1) {
				Color bgColor;
				if (isRulesPage) {
					bgColor = Color.ParseColor (GetString (Resource.Color.rules));
					welcomeText.Text = "Rules";
				} else {
					bgColor = Color.ParseColor (GetString (Resource.Color.blog));
					welcomeText.Text = "Blog";
				}

				colorBlock.Background = new ColorDrawable (bgColor);
			}
			webView = v.FindViewById <WebView> (Resource.Id.webview);

			return v;
		}
	}
}

