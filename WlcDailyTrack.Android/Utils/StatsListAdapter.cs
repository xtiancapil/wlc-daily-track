using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using Android.Text;

namespace WlcDailyTrackAndroid
{
	public class StatsListAdapter : ArrayAdapter<Core.Stat>
	{
		private LayoutInflater inflater;
		private Core.Stat stat;
		private int point = 0;

		private Dictionary<string, int> CategoryIcons { get; set; }
		private LinearLayout.LayoutParams lp;
		public List<Core.Stat> Stats { get; set; }

		public StatsListAdapter (Context context) : base (context, 0)
		{
			inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			Stats = new List<Core.Stat> ();
			CategoryIcons = new Dictionary<string, int> ();
			// TODO: move this to app start / config section

			CategoryIcons.Add ("Nutrition", Resource.Drawable.nutrition_light);
			CategoryIcons.Add ("Workout", Resource.Drawable.workout_light);
			CategoryIcons.Add ("Mobilize", Resource.Drawable.mobilize_light);
			CategoryIcons.Add ("Bonus", Resource.Drawable.bonus_light);
			CategoryIcons.Add ("Reflections", Resource.Drawable.bonus_light);
			CategoryIcons.Add ("Supplement", Resource.Drawable.supplement_light);
			CategoryIcons.Add ("Water", Resource.Drawable.water_light);
			CategoryIcons.Add ("Lifestyle", Resource.Drawable.lifestyle_light);

			lp = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, 1);
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null) {
				convertView = inflater.Inflate(Resource.Layout.list_item_stat, parent, false);
				holder = new ViewHolder ();
				holder.titleView = convertView.FindViewById<TextView> (Resource.Id.pointView);
				holder.timeView = convertView.FindViewById<TextView> (Resource.Id.dateTimeView);
				holder.scoreLayout = convertView.FindViewById<LinearLayout> (Resource.Id.scoreLayout);
				holder.titleView.SetTextColor (Android.Graphics.Color.White);
				convertView.Tag = holder;
			} else {
				holder = (ViewHolder)convertView.Tag;
			}

			stat = Stats [position];

			//point = stat.Points ["Total"];//stat.Points.TryGetValue ("Total", out point);

			stat.Points.TryGetValue ("Bonus", out point);
			point += stat.Points ["Total"];

			holder.titleView.Text = point.ToString ();

			holder.timeView.TextFormatted = Html.FromHtml(string.Format("<b>Day {0}:</b> {1}", stat.Day, stat.StatDate.ToLongDateString ()));
			holder.scoreLayout.RemoveAllViews ();
			foreach (var score in stat.Points) {
				if (!score.Key.Equals ("Total") && !score.Key.Equals ("Reflections Written")) {
					var linearLayout = new LinearLayout (parent.Context);
					var textView = new TextView (parent.Context);
					var iconView = new ImageView (parent.Context);
					textView.Text = score.Value.ToString ();
					textView.Gravity = GravityFlags.CenterHorizontal;
					int _categoryIcon = -1;
					CategoryIcons.TryGetValue (score.Key, out _categoryIcon);
					if(_categoryIcon != -1) {
						iconView.SetImageResource (_categoryIcon);
					}
					linearLayout.Orientation = Orientation.Vertical;
					linearLayout.LayoutParameters = lp;
					linearLayout.AddView (textView);
					linearLayout.AddView (iconView);
					holder.scoreLayout.AddView (linearLayout);
				}
			}
//			stat = null;

			return convertView;
		}

		public override int Count {
			get {
				return Stats.Count;
			}
		}
		//pointView
		public class ViewHolder : Java.Lang.Object {
			public LinearLayout scoreLayout;
			public TextView timeView;
			public TextView titleView;
			public TextView speakerName;
		}

	}
}

