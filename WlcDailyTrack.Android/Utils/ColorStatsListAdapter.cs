using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using Android.Text;

namespace WlcDailyTrackAndroid
{
	public class ColorStatsListAdapter : ArrayAdapter<Core.Stat>
	{
		private LayoutInflater inflater;
		private Core.Stat stat;
		private int point = 0;

		private Dictionary<string, int> CategoryColors { get; set; }
		private Dictionary<string, int> CategoryResourceIds { get; set; }
		private LinearLayout.LayoutParams lp;
		public List<Core.Stat> Stats { get; set; }

		public ColorStatsListAdapter (Context context) : base (context, 0)
		{
			inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			Stats = new List<Core.Stat> ();

			CategoryColors = new Dictionary<string, int> ();
			CategoryResourceIds = new Dictionary<string, int> ();

			// TODO: move this to app start / config section
//			CategoryColors.Add ("Nutrition", Resource.Color.nutrition);
//			CategoryColors.Add ("Workout", Resource.Color.workout);
//			CategoryColors.Add ("Mobilize", Resource.Color.mobilize);
//			CategoryColors.Add ("Bonus", Resource.Color.bonus);
//			CategoryColors.Add ("Reflections", Resource.Color.reflection);
//			CategoryColors.Add ("Supplement", Resource.Color.supplement);
//			CategoryColors.Add ("Water", Resource.Color.water);
//			CategoryColors.Add ("Lifestyle", Resource.Color.lifestyle);
//
			CategoryColors.Add ("Nutrition", Resource.Drawable.bg_nutrition);
			CategoryColors.Add ("Workout", Resource.Drawable.bg_workout);
			CategoryColors.Add ("Mobilize", Resource.Drawable.bg_mobilize);
			CategoryColors.Add ("Reflection", Resource.Drawable.bg_reflection);
			CategoryColors.Add ("Supplement", Resource.Drawable.bg_supplement);
			CategoryColors.Add ("Water", Resource.Drawable.bg_water);
			CategoryColors.Add ("Lifestyle", Resource.Drawable.bg_lifestyle);

			CategoryResourceIds.Add ("Nutrition", Resource.Id.Nutrition);
			CategoryResourceIds.Add ("Workout", Resource.Id.Workout);
			CategoryResourceIds.Add ("Mobilize", Resource.Id.Mobilize);
			CategoryResourceIds.Add ("Reflection", Resource.Id.Reflection);
			CategoryResourceIds.Add ("Supplement", Resource.Id.Supplement);
			CategoryResourceIds.Add ("Water", Resource.Id.Water);
			CategoryResourceIds.Add ("Lifestyle", Resource.Id.Lifestyle);

			lp = new LinearLayout.LayoutParams (0, 40, 1);
		}


		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null) {
				convertView = inflater.Inflate(Resource.Layout.list_item_stat_color, parent, false);
				holder = new ViewHolder ();
				holder.scoreLayout = convertView.FindViewById<LinearLayout> (Resource.Id.scoreLayout);
				holder.pointViews.Add ("Nutrition", convertView.FindViewById<TextView> (Resource.Id.nutritionScore));
				holder.pointViews.Add ("Workout", convertView.FindViewById<TextView> (Resource.Id.workoutScore));
				holder.pointViews.Add ("Mobilize", convertView.FindViewById<TextView> (Resource.Id.mobilizeScore));
				holder.pointViews.Add ("Reflection", convertView.FindViewById<TextView> (Resource.Id.reflectionScore));
				holder.pointViews.Add ("Supplement", convertView.FindViewById<TextView> (Resource.Id.supplementScore));
				holder.pointViews.Add ("Water", convertView.FindViewById<TextView> (Resource.Id.waterScore));
				holder.pointViews.Add ("Lifestyle", convertView.FindViewById<TextView> (Resource.Id.lifestyleScore));

				convertView.Tag = holder;
			} else {
				holder = (ViewHolder)convertView.Tag;
			}

			stat = Stats [position];

			//point = stat.Points ["Total"];//stat.Points.TryGetValue ("Total", out point);

			stat.Points.TryGetValue ("Bonus", out point);
			point += stat.Points ["Total"];
			//holder.scoreLayout.RemoveAllViews ();
			foreach (var score in stat.Points) {
				if (!score.Key.Equals ("Total") && !score.Key.Equals ("Reflections Written")) {

					int categoryColor = -1;
					CategoryColors.TryGetValue (score.Key, out categoryColor);
					int categoryId = -1;
					CategoryResourceIds.TryGetValue (score.Key, out categoryId);
					convertView.FindViewById<RelativeLayout>(categoryId).SetBackgroundResource (Resource.Drawable.bg_layout);
					if (categoryColor != -1 && categoryId != -1) {
						holder.pointViews [score.Key].Text = score.Value.ToString ();
						holder.pointViews [score.Key].SetTextColor (Android.Graphics.Color.LightGray);
						if (score.Value > 0) {
							holder.pointViews [score.Key].SetTextColor (Android.Graphics.Color.White);
							convertView.FindViewById<RelativeLayout>(categoryId).SetBackgroundResource (categoryColor);
							if (score.Value != 5 && "Nutrition".Equals (score.Key, StringComparison.OrdinalIgnoreCase)) {
								convertView.FindViewById<RelativeLayout>(categoryId).SetBackgroundResource (Resource.Drawable.bg_nutrition_partial);
							}


						}
					}

				}
			}

			return convertView;
		}

		public override int Count {
			get {
				return Stats.Count;
			}
		}
		//pointView
		public class ViewHolder : Java.Lang.Object {
			public ViewHolder () {
				pointViews = new Dictionary<string, TextView>();
			}
			public LinearLayout scoreLayout;
			public Dictionary<string, TextView> pointViews;
		}

	}
}

