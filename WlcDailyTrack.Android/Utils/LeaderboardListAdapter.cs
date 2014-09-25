using System;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using Android.Widget;

namespace WlcDailyTrackAndroid
{
	public class LeaderboardListAdapter : ArrayAdapter<Core.LeaderboardItem>
	{
		private LayoutInflater inflater;
		private Core.LeaderboardItem player;
		public List<Core.LeaderboardItem> Leaderboard { get; set; }
		public LeaderboardListAdapter (Context context) : base (context, 0)
		{
			inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			Leaderboard = new List<Core.LeaderboardItem>();
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null) {
				convertView = inflater.Inflate (Resource.Layout.list_item_leaderboard, parent, false);
				holder = new ViewHolder ();

				holder.leaderboardItemLayout = convertView.FindViewById<LinearLayout> (Resource.Id.leaderboardLayout);
				holder.rankTextView = convertView.FindViewById<TextView> (Resource.Id.rankTextView);
				holder.playerTextView = convertView.FindViewById<TextView> (Resource.Id.playerTextView);
				holder.teamTextView = convertView.FindViewById<TextView> (Resource.Id.teamTextView);
				holder.totalTextView = convertView.FindViewById<TextView> (Resource.Id.totalTextView);

				convertView.Tag = holder;
			} else {
				holder = (ViewHolder)convertView.Tag;
			}

			player = Leaderboard [position];

			holder.rankTextView.Text = player.Rank.ToString ();
			holder.playerTextView.Text = player.Player;
			holder.teamTextView.Text = player.Teams;
			holder.totalTextView.Text = player.Total.ToString ();

			return convertView;
		}

		public override int Count {
			get {
				return Leaderboard.Count;
			}
		}

		public class ViewHolder : Java.Lang.Object {
			public LinearLayout leaderboardItemLayout;
			public TextView rankTextView;
			public TextView playerTextView;
			public TextView teamTextView;
			public TextView totalTextView;
		}
	}
}

