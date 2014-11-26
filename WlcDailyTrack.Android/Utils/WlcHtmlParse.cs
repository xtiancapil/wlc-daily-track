using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;

namespace WlcDailyTrackAndroid
{
	/// <summary>
	/// Wlc html parse. Methods for parsing the different HTML formats needed.
	/// </summary>
	public class WlcHtmlParse
	{
		private HtmlDocument doc;

		public WlcHtmlParse ()
		{
			doc = new HtmlDocument ();
		}

		public List<Core.Stat>  GetStats(string statsString) {
			List<Core.Stat> myStats = new List<Core.Stat> ();

			if (string.IsNullOrEmpty (statsString)) {
				return myStats;
			}
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

				// only want to display up to the current date only.
				if (dayStat.StatDate.Date.CompareTo (DateTime.Now.Date) > 0) {
					continue;
				}

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

			return myStats;
		}
	}
}

