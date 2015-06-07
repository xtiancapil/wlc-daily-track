using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Core;
using Humanizer;
using Squareup.Picasso;
using System.Globalization;

namespace CounterWeightsDroid
{
	public class ReflectionAdapter : ArrayAdapter<Feed>
	{
		public Feed ReflectionFeed { get; set; }

		LayoutInflater inflater;
		Reflection reflection;
		DateTime postDate;

		public ReflectionAdapter (Context context) : base(context, 0)
		{
			inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			ReflectionFeed = new Feed ();
		}

		XamSvg.PictureBitmapDrawable img;
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null) {
				convertView = inflater.Inflate (Resource.Layout.list_item_reflection, parent, false);
				holder = new ViewHolder ();
				holder.username = convertView.FindViewById<TextView> (Resource.Id.username);
				holder.postDate = convertView.FindViewById<TextView> (Resource.Id.postDate);
				holder.comment = convertView.FindViewById<TextView> (Resource.Id.reflectionText);
				holder.comments = convertView.FindViewById<TextView> (Resource.Id.commentText);
				holder.likes = convertView.FindViewById<TextView> (Resource.Id.likeText);
				holder.thumbnail = convertView.FindViewById<ImageView> (Resource.Id.userImage);
				convertView.Tag = holder;
			} else {
				holder = (ViewHolder)convertView.Tag;
			}

			reflection = ReflectionFeed.data [position];

			Picasso.With (Context).Load (reflection.user.photo).Into (holder.thumbnail);
//			img = XamSvg.SvgFactory.GetDrawable (Context.Resources, Resource.Raw.nutrition);
//			holder.thumbnail.SetImageDrawable (img);
//			holder.thumbnail.SetBackgroundColor (Android.Graphics.Color.Red);
			Console.WriteLine("{0} - {1}",reflection.user.full_name, reflection.created_at);

			try {

			// sample date time 2015-06-06T06:26:04-07:00
				Console.WriteLine(reflection.created_at);

				var zoned = NodaTime.ZonedDateTime.FromDateTimeOffset(DateTimeOffset.Parse(reflection.created_at));
				var dto = zoned.LocalDateTime.ToDateTimeUnspecified();
				Console.WriteLine("datetiime {0}", dto);
				Console.WriteLine("nodattime {0}", zoned.LocalDateTime);

				holder.postDate.Text =  dto.Humanize(false, null, null);//postDate.Humanize(false,null ,null );
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
			holder.username.Text = reflection.user.full_name;
			// (false, DateTime.Now, System.Globalization.CultureInfo.CurrentCulture);// postDate.ToLocalTime ().ToString();//reflection.created_at;
			holder.comment.Text = reflection.content;
			holder.comments.Text = string.Format ("{0} comments", reflection.comments.Count);
			holder.likes.Text = string.Format ("{0} likes", reflection.likes.Count);

			return convertView;
		}

		public override int Count {
			get {
				return ReflectionFeed.data.Count;
			}
		}

		class ViewHolder : Java.Lang.Object {
			public TextView username;
			public TextView postDate;
			public TextView comment;
			public TextView likes;
			public TextView comments;
			public ImageView thumbnail;
		}
	}
}

