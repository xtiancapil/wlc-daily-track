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

	public class Team {
		public int id {
			get;
			set;
		}

		public string slug {
			get;
			set;
		}

		public string name {
			get;
			set;
		}

		public int size {
			get;
			set;
		}
	}

	public class DailyRecord {

		public DailyRecord () {
			scores = new List<Score> ();
		}

		public object id { get; set; }
		public string recorded_for { get; set; }
		public bool can_record { get; set; }
		public int day { get; set; }
		public string title { get; set; }
		public object retroactive { get; set; }
		public bool recording_open { get; set; }
		public bool recorded { get; set; }
		public List<Score> scores { get; set; }
		public ChallengeProfile challenge_profile { get; set; }
		public Reflection reflection { get; set; }
		public string state { get; set; }
		public bool showSummary { get; set; }
		public string editWindow { get; set; }
	}

	public class Reflection
	{
		public Reflection () {
			likes = new List<object> ();
			comments = new List<Comment> ();
			scopes = new List<Scope> ();
		}

		public int id { get; set; }
		public string content { get; set; }
		public int challenge_profile_id { get; set; }
		public int user_id { get; set; }
		public string type { get; set; }
		public object image { get; set; }
		public string postable_type { get; set; }
		public int postable_id { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }
		public bool pinned { get; set; }
		public User user { get; set; }
		public List<Comment> comments { get; set; }
		public List<object> likes { get; set; }
		public List<Scope> scopes { get; set; }
	}

	public class Scope
	{
		public int scope_id { get; set; }
		public string scope_type { get; set; }
	}


	public class ChallengeLevel
	{
		public int id { get; set; }
		public string name { get; set; }
	}

	public class ChallengeProfile
	{
		public ChallengeProfile() {
			incomplete_personal_info = new List<object> ();
			missed_daily_record_dates = new List<string> ();
			teams = new List<Team> ();
		}
		public int id { get; set; }
		public bool should_record_missing_daily_records { get; set; }
		public List<string> missed_daily_record_dates { get; set; }
		public bool has_missing_daily_records { get; set; }
		public int mulligans_remaining { get; set; }
		public bool can_record_todays_daily_record { get; set; }
		public bool can_record_yesterdays_daily_record { get; set; }
		public object can_edit_yesterdays_daily_record { get; set; }
		public bool daily_recording_ended { get; set; }
		public string today { get; set; }
		public string yesterday { get; set; }
		public int indulgence_tokens { get; set; }
		public int rest_day_tokens { get; set; }
		public int free_day_tokens { get; set; }
		public bool has_bonus_tokens { get; set; }
		public double average_score { get; set; }
		public int total_score { get; set; }
		public bool can_change_challenge_level { get; set; }
		public string level_locks_on { get; set; }
		public double workout_improvement_percent { get; set; }
		public double body_measurement_improvement_percent { get; set; }
		public bool personal_info_incomplete { get; set; }
		public List<object> incomplete_personal_info { get; set; }
		public ChallengeLevel challenge_level { get; set; }
		public List<Team> teams { get; set; }
		public bool challenge_started { get; set; }
		public User user { get; set; }
		public object preliminary_score { get; set; }
		public object final_score { get; set; }
	}

	public class User
	{
		public int id { get; set; }
		public string full_name { get; set; }
		public string photo { get; set; }
		public string birth_date { get; set; }
		public string gender { get; set; }
		public string country { get; set; }
		public string state { get; set; }
		public string city { get; set; }
		public string short_biography { get; set; }
		public string time_zone { get; set; }
		public string email { get; set; }
		public string photo_medium { get; set; }
		public string small_square_thumbnail { get; set; }
		public string thumbnail { get; set; }
		public PrivacySettings privacy_settings { get; set; }
	}

	public class PrivacySettings
	{
		public bool show_email { get; set; }
		public bool show_birthday { get; set; }
		public bool show_about_me { get; set; }
		public bool show_body_measurements { get; set; }
		public bool show_workout_results { get; set; }
		public bool notify_comment_email { get; set; }
		public bool receive_marketing { get; set; }
		public bool show_photo_before_after { get; set; }
		public bool email_reminder { get; set; }
		public bool notify_comment_reply_email { get; set; }
		public bool show_reflections { get; set; }
	}

	public class YesNoOption
	{
		public string label { get; set; }
		public int value { get; set; }
	}

	public class ChallengeCategory
	{
		public int id { get; set; }
		public string name { get; set; }
		public int max_score { get; set; }
		public bool incremental { get; set; }
		public string display_name { get; set; }
	}

	public class Score
	{
		public Score () {
			incremental_options = new List<int> ();
			yes_no_options = new List<YesNoOption> ();
		}

		public object id { get; set; }
		public int challenge_category_id { get; set; }
		public int points { get; set; }
		public string name { get; set; }
		public string display_name { get; set; }
		public bool incremental { get; set; }
		public List<int> incremental_options { get; set; }
		public List<YesNoOption> yes_no_options { get; set; }
		public ChallengeCategory challenge_category { get; set; }
	}

	public class Comment
	{
		public int id { get; set; }
		public string body { get; set; }
		public int challenge_profile_id { get; set; }
		public DateTime created_at { get; set; }
		public User user { get; set; }
	}

	public class Pagination
	{
		public int next_cursor { get; set; }
		public int size { get; set; }
	}

	public class Feed {
		public Feed () {
			data = new List<Reflection> ();
		}

		public List<Reflection> data { get; set; }
		public Pagination pagination { get; set; }

	}
}

