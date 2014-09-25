using System;
using System.Collections.Generic;

namespace Core
{
	public class Stat
	{
		public Stat ()
		{
			Points = new Dictionary<string, int> ();		
		}

		public int Day {
			get;
			set;
		}

		public DateTime StatDate {
			get; 
			set;
		}

		public Dictionary<string,int> Points {
			get;
			set;
		}
	}

	public class LeaderboardItem {
		public int Rank {
			get;
			set;
		}

		public int Total {
			get;
			set;
		}

		public string Player {
			get;
			set;
		}

		public string Teams {
			get;
			set;
		}
	}
}

