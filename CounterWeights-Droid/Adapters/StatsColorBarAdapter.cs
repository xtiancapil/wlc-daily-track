using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using Android.Text;

namespace CounterWeightsDroid
{
	public class StatsColorBarAdapter : ArrayAdapter<Core.Stat>
	{
		private LayoutInflater inflater;
		private Core.Stat stat;
		private int point = 0;

		private Dictionary<string, int> CategoryColors { get; set; }
		private Dictionary<string, int> CategoryResourceIds { get; set; }
		private LinearLayout.LayoutParams lp;
		public List<Core.Stat> Stats { get; set; }

		public StatsColorBarAdapter (Context context) : base (context, 0)
		{
			inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			Stats = new List<Core.Stat> ();

			CategoryColors = new Dictionary<string, int> ();
			CategoryResourceIds = new Dictionary<string, int> ();

			//			CategoryColors.Add ("Nutrition", Resource.Drawable.bg_nutrition);
			//			CategoryColors.Add ("Workout", Resource.Drawable.bg_workout);
			//			CategoryColors.Add ("Mobilize", Resource.Drawable.bg_mobilize);
			//			CategoryColors.Add ("Reflection", Resource.Drawable.bg_reflection);
			//			CategoryColors.Add ("Supplement", Resource.Drawable.bg_supplement);
			//			CategoryColors.Add ("Water", Resource.Drawable.bg_water);
			//			CategoryColors.Add ("Lifestyle", Resource.Drawable.bg_lifestyle);
			//
			//			CategoryResourceIds.Add ("Nutrition", Resource.Id.Nutrition);
			//			CategoryResourceIds.Add ("Workout", Resource.Id.Workout);
			//			CategoryResourceIds.Add ("Mobilize", Resource.Id.Mobilize);
			//			CategoryResourceIds.Add ("Reflection", Resource.Id.Reflection);
			//			CategoryResourceIds.Add ("Supplement", Resource.Id.Supplement);
			//			CategoryResourceIds.Add ("Water", Resource.Id.Water);
			//			CategoryResourceIds.Add ("Lifestyle", Resource.Id.Lifestyle);

			//lp = new LinearLayout.LayoutParams (0, 40, 1);
		}

		float pct, pnt;
		ViewGroup.LayoutParams textViewLp;
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null) {
				convertView = inflater.Inflate(Resource.Layout.list_item_color_bar, parent, false);
				holder = new ViewHolder ();
				holder.emptyBarView = convertView.FindViewById<LinearLayout> (Resource.Id.emptyView);
				holder.pointView = convertView.FindViewById<TextView> (Resource.Id.pointView);
				holder.totalView = convertView.FindViewById<TextView> (Resource.Id.totalView);
				holder.nutrition = convertView.FindViewById<TextView> (Resource.Id.nutritionScore);
				holder.workout = convertView.FindViewById<TextView> (Resource.Id.exerciseScore);
				holder.stretch = convertView.FindViewById<TextView> (Resource.Id.stretchingScore);
				holder.supplement = convertView.FindViewById<TextView> (Resource.Id.supplementScore);
				holder.water = convertView.FindViewById<TextView> (Resource.Id.waterScore);
				holder.lifestyle = convertView.FindViewById<TextView> (Resource.Id.lifestyleScore);
				holder.reflection = convertView.FindViewById<TextView> (Resource.Id.reflectionScore);
				convertView.Tag = holder;
			} else {
				holder = (ViewHolder)convertView.Tag;
			}

			stat = Stats [position];

			holder.pointView.Text = stat.StatDate.ToString ("D");//.ToString ("MM/dd/yy");// stat.Day.ToString ();
			holder.totalView.Text = string.Format ("{0} out of {1} pts", stat.Points ["Total"], 13);
			//point = stat.Points ["Total"];//stat.Points.TryGetValue ("Total", out point);

			stat.Points.TryGetValue ("Bonus", out point);
			point += stat.Points ["Total"];
			pct = (point / 13.0f);
			pnt = (stat.Points ["Nutrition"] / 13.0f);
			holder.nutrition.Text = stat.Points ["Nutrition"].ToString ();
			lp = holder.nutrition.LayoutParameters as LinearLayout.LayoutParams;
			holder.nutrition.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, pnt);

			pnt = (stat.Points ["Workout"] / 13.0f);
			holder.workout.Text = stat.Points ["Workout"].ToString ();
			lp = holder.workout.LayoutParameters as LinearLayout.LayoutParams;
			holder.workout.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, pnt);

			pnt = (stat.Points ["Mobilize"] / 13.0f);
			holder.stretch.Text = stat.Points ["Mobilize"].ToString ();
			lp = holder.stretch.LayoutParameters as LinearLayout.LayoutParams;
			holder.stretch.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, pnt);

			pnt = (stat.Points ["Supplement"] / 13.0f);
			holder.supplement.Text = stat.Points ["Supplement"].ToString ();
			lp = holder.supplement.LayoutParameters as LinearLayout.LayoutParams;
			holder.supplement.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, pnt);

			pnt = (stat.Points ["Water"] / 13.0f);
			holder.water.Text = stat.Points ["Water"].ToString ();
			lp = holder.water.LayoutParameters as LinearLayout.LayoutParams;
			holder.water.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, pnt);

			pnt = (stat.Points ["Lifestyle"] / 13.0f);
			holder.lifestyle.Text = stat.Points ["Lifestyle"].ToString ();
			lp = holder.lifestyle.LayoutParameters as LinearLayout.LayoutParams;
			holder.lifestyle.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, pnt);

			pnt = (stat.Points ["Reflection"] / 13.0f);
			holder.reflection.Text = stat.Points ["Reflection"].ToString ();
			lp = holder.reflection.LayoutParameters as LinearLayout.LayoutParams;
			holder.reflection.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, pnt);

			lp = holder.emptyBarView.LayoutParameters as LinearLayout.LayoutParams;
			holder.emptyBarView.LayoutParameters = new LinearLayout.LayoutParams(lp.Width, lp.Height, (1.0f - pct));

			return convertView;
		}

		public override int Count {
			get {
				return Stats.Count;
			}
		}

		public class ViewHolder : Java.Lang.Object {
			public ViewHolder () {
				pointViews = new Dictionary<string, TextView>();
			}
			public LinearLayout barView;
			public LinearLayout emptyBarView;
			public TextView pointView;
			public TextView totalView;
			public LinearLayout scoreLayout;
			public Dictionary<string, TextView> pointViews;

			public TextView nutrition;
			public TextView workout;
			public TextView stretch;
			public TextView supplement;
			public TextView water;
			public TextView lifestyle;
			public TextView reflection;

		}

	}
}

