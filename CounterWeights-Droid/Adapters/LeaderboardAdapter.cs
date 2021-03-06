﻿using System;
using Android.Widget;
using Core.Leaderboard;
using Android.Views;
using Android.Content;
//using XamSvg;
using Android.Graphics.Drawables;

namespace CounterWeightsDroid
{
	public class LeaderboardAdapter : ArrayAdapter<Leaderboard>
	{

		public Leaderboard Leaderboard { get; set; }

		LayoutInflater inflater;

		public LeaderboardAdapter (Context context) : base(context, 0)
		{
			inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			Leaderboard = new Leaderboard ();
		}

		Member member;

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null) {
				convertView = inflater.Inflate (Resource.Layout.list_item_leaderboard_rank, parent, false);
				holder = new ViewHolder ();
				holder.rank = convertView.FindViewById<TextView> (Resource.Id.rank);
				holder.user = convertView.FindViewById<TextView> (Resource.Id.memberName);
				holder.nutrition = convertView.FindViewById<TextView> (Resource.Id.nutritionScore);
				holder.exercise = convertView.FindViewById<TextView> (Resource.Id.exerciseScore);
				holder.stretching = convertView.FindViewById<TextView> (Resource.Id.stretchingScore);
				holder.lifestyle = convertView.FindViewById<TextView> (Resource.Id.lifestyleScore);
				holder.reflection = convertView.FindViewById<TextView> (Resource.Id.reflectionScore);
				holder.water = convertView.FindViewById<TextView> (Resource.Id.waterScore);
				holder.totalScore = convertView.FindViewById<TextView> (Resource.Id.totalScore);
				holder.supplement = convertView.FindViewById<TextView> (Resource.Id.supplementScore);

				float density = convertView.Resources.DisplayMetrics.Density;
				int objectToDrawHeight = 96 * (int) density;


				//var svg = SvgFactory.GetBitmap (convertView.Context.Resources, Resource.Raw.nutrition, objectToDrawHeight, objectToDrawHeight, XamSvg.SvgColorMapperFactory.FromFunc ((g) => {
				//	return convertView.Resources.GetColor (Resource.Color.nutrition);
				//}));
				//holder.nutrition.SetCompoundDrawablesRelativeWithIntrinsicBounds (null, new BitmapDrawable (svg), null, null);

				//svg = SvgFactory.GetBitmap (convertView.Context.Resources, Resource.Raw.exercise, objectToDrawHeight, objectToDrawHeight, XamSvg.SvgColorMapperFactory.FromFunc ((g) => {
				//	return convertView.Resources.GetColor (Resource.Color.workout);
				//}));
				//holder.exercise.SetCompoundDrawablesRelativeWithIntrinsicBounds (null, new BitmapDrawable (svg), null, null);

				//svg = SvgFactory.GetBitmap (convertView.Context.Resources, Resource.Raw.stretching, objectToDrawHeight, objectToDrawHeight, XamSvg.SvgColorMapperFactory.FromFunc ((g) => {
				//	return convertView.Resources.GetColor (Resource.Color.stretch);
				//}));
				//holder.stretching.SetCompoundDrawablesRelativeWithIntrinsicBounds (null, new BitmapDrawable (svg), null, null);

				//svg = SvgFactory.GetBitmap (convertView.Context.Resources, Resource.Raw.lifestyle, objectToDrawHeight, objectToDrawHeight, XamSvg.SvgColorMapperFactory.FromFunc ((g) => {
				//	return convertView.Resources.GetColor (Resource.Color.lifestyle);
				//}));
				//holder.lifestyle.SetCompoundDrawablesRelativeWithIntrinsicBounds (null, new BitmapDrawable (svg), null, null);

				//svg = SvgFactory.GetBitmap (convertView.Context.Resources, Resource.Raw.reflection, objectToDrawHeight, objectToDrawHeight, XamSvg.SvgColorMapperFactory.FromFunc ((g) => {
				//	return convertView.Resources.GetColor (Resource.Color.reflection);
				//}));
				//holder.reflection.SetCompoundDrawablesRelativeWithIntrinsicBounds (null, new BitmapDrawable (svg), null, null);

				//svg = SvgFactory.GetBitmap (convertView.Context.Resources, Resource.Raw.water, objectToDrawHeight, objectToDrawHeight, XamSvg.SvgColorMapperFactory.FromFunc ((g) => {
				//	return convertView.Resources.GetColor (Resource.Color.water);
				//}));
				//holder.water.SetCompoundDrawablesRelativeWithIntrinsicBounds (null, new BitmapDrawable (svg), null, null);

				//svg = SvgFactory.GetBitmap (convertView.Context.Resources, Resource.Raw.supplement, objectToDrawHeight, objectToDrawHeight, XamSvg.SvgColorMapperFactory.FromFunc ((g) => {
				//	return convertView.Resources.GetColor (Resource.Color.supplement);
				//}));
				//holder.supplement.SetCompoundDrawablesRelativeWithIntrinsicBounds (null, new BitmapDrawable (svg), null, null);
				convertView.Tag = holder;
			} else {
				holder = (ViewHolder) convertView.Tag;
			}

			member = Leaderboard.leaderboards [position];

			holder.rank.Text = member.total_score_rank.ToString ();
			holder.user.Text = member.full_name;
			holder.nutrition.Text = member.scores.nutrition.ToString ();
			holder.exercise.Text = member.scores.workout.ToString ();
			holder.stretching.Text = member.scores.mobilize.ToString ();
			holder.lifestyle.Text = member.scores.lifestyle.ToString ();
			holder.reflection.Text = member.scores.reflection.ToString ();
			holder.water.Text = member.scores.water.ToString ();
			holder.totalScore.Text = member.total_score.ToString ();
			holder.supplement.Text = member.scores.supplement.ToString ();

			return convertView;
		}

		public override int Count {
			get {
				return Leaderboard.leaderboards.Count;
			}
		}

		class ViewHolder : Java.Lang.Object {
			public TextView rank { get; set; }
			public TextView user { get; set; }
			public TextView nutrition { get; set; }
			public TextView exercise { get; set; }
			public TextView stretching { get; set; }
			public TextView supplement { get; set; }
			public TextView water { get; set; }
			public TextView lifestyle { get; set; }
			public TextView reflection { get; set; }
			public TextView totalScore { get; set; }
		}
	}
}

