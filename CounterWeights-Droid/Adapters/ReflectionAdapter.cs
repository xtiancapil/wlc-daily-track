using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Core;

namespace CounterWeightsDroid
{
	public class ReflectionAdapter : ArrayAdapter<Feed>
	{
		public Feed ReflectionFeed { get; set; }

		LayoutInflater inflater;
		Reflection reflection;

		public ReflectionAdapter (Context context) : base(context, 0)
		{
			inflater = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
			ReflectionFeed = new Feed ();
		}

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

				convertView.Tag = holder;
			} else {
				holder = (ViewHolder)convertView.Tag;
			}

			reflection = ReflectionFeed.data [position];

			holder.username.Text = reflection.user.full_name;
			holder.postDate.Text = reflection.created_at;
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
		}
	}
}

