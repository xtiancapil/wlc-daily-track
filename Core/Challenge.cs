using System;

namespace Core
{
	public class Challenge
	{
		public Challenge (DateTime start, DateTime end)
		{
			StartDate = start;
			EndDate = end;
		}

		public DateTime StartDate;
		public DateTime EndDate;
	}
}

