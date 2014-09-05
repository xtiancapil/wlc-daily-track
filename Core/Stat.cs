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
}

