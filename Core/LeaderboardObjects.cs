using System;
using System.Collections.Generic;

namespace Core.Leaderboard
{
	public class Leaderboard
	{
		public Leaderboard () {
			leaderboards = new List<Member> ();
		}
		public List<Member> leaderboards { get; set; }
		public Pagination pagination { get; set; }
	}

	public class Member
	{
		public int challenge_profile_id { get; set; }
		public int user_id { get; set; }
		public string full_name { get; set; }
		public int total_score_rank { get; set; }
		public int total_score { get; set; }
		public Scores scores { get; set; }
	}

	public class Scores
	{
		public int lifestyle { get; set; }
		public int mobilize { get; set; }
		public int nutrition { get; set; }
		public int reflection { get; set; }
		public int supplement { get; set; }
		public int water { get; set; }
		public int workout { get; set; }
	}

	public class Pagination
	{
		public int page { get; set; }
		public int size { get; set; }
		public int total { get; set; }
	}

}

