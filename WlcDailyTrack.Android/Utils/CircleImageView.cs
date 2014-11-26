
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace WlcDailyTrackAndroid
{
	public class CircleImageView : ImageView
	{
		private static ScaleType SCALE_TYPE = ScaleType.CenterCrop;

		private static Bitmap.Config BITMAP_CONFIG = Bitmap.Config.Argb8888;
		private static int COLORDRAWABLE_DIMENSION = 1;

		private static int DEFAULT_BORDER_WIDTH = 0;
		private static Color DEFAULT_BORDER_COLOR = Color.Black;

		private RectF mDrawableRect = new RectF();
		private RectF mBorderRect = new RectF();

		private Matrix mShaderMatrix = new Matrix();
		private Paint mBitmapPaint = new Paint();
		private Paint mBorderPaint = new Paint();

		private Color mBorderColor = DEFAULT_BORDER_COLOR;
		private int mBorderWidth = DEFAULT_BORDER_WIDTH;

		private Bitmap mBitmap;
		private BitmapShader mBitmapShader;
		private int mBitmapWidth;
		private int mBitmapHeight;

		private float mDrawableRadius;
		private float mBorderRadius;

		private bool mReady;
		private bool mSetupPending;

		public CircleImageView (IntPtr javaReference, JniHandleOwnership transfer) : 
			base (javaReference, transfer) 
		{
		}

		public CircleImageView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public CircleImageView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public CircleImageView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{

			TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.CircleImageView, defStyle, 0);

			mBorderWidth = a.GetDimensionPixelSize(Resource.Styleable.CircleImageView_border_width, DEFAULT_BORDER_WIDTH);
			mBorderColor = a.GetColor(Resource.Styleable.CircleImageView_border_color, DEFAULT_BORDER_COLOR);

			a.Recycle ();

			Initialize ();
		}

		void Initialize ()
		{
			base.SetScaleType (SCALE_TYPE);
			mReady = true;
			if (mSetupPending) {
				Setup ();
				mSetupPending = false;
			}
		}

		public override ScaleType GetScaleType ()
		{
			return SCALE_TYPE;
		}

		public override void SetScaleType (ScaleType scaleType)
		{
			if (scaleType != SCALE_TYPE) {
				throw new Java.Lang.IllegalArgumentException (String.Format ("ScaleType %0 not supported", scaleType));
			}

			//base.SetScaleType (scaleType);
		}

		protected override void OnDraw (Canvas canvas)
		{
			if (Drawable == null) {
				return;
			}

			canvas.DrawCircle (Width / 2, Height / 2, mDrawableRadius, mBitmapPaint);
			if (mBorderWidth != 0) {
				canvas.DrawCircle (Width / 2, Height / 2, mBorderRadius, mBorderPaint);
			}

		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			Setup ();
		}

		public Color BorderColor {
			get { return mBorderColor; }
			set {
				if (value == mBorderColor) {
					return;
				}

				mBorderColor = value;
				mBorderPaint.Color = mBorderColor;
				Invalidate();
			}
		}

		public int BorderWidth {
			get { return mBorderWidth; }
			set {
				if (value == mBorderWidth) {
					return;
				}

				mBorderWidth = value;
				Setup ();
			}
		}

		public override void SetImageBitmap (Bitmap bm)
		{
			base.SetImageBitmap (bm);
			mBitmap = bm;
			Setup ();
		}

		public override void SetImageDrawable (Android.Graphics.Drawables.Drawable drawable)
		{
			base.SetImageDrawable (drawable);
			mBitmap = GetBitMapFromDrawable (drawable);
			Setup ();
		}

		public override void SetImageResource (int resId)
		{
			base.SetImageResource (resId);
			mBitmap = GetBitMapFromDrawable (Drawable);
			Setup ();
		}

		public override void SetImageURI (Android.Net.Uri uri)
		{
			base.SetImageURI (uri);
			mBitmap = GetBitMapFromDrawable (Drawable);
			Setup ();
		}

		private Bitmap GetBitMapFromDrawable(Android.Graphics.Drawables.Drawable drawable) {
			if (drawable == null) {
				return null;
			}

			if (drawable.GetType() == typeof(BitmapDrawable)) {
				return ((BitmapDrawable) drawable).Bitmap;
			}

			try {
				Bitmap bitmap;
				if(drawable.GetType() == typeof(ColorDrawable)) {
					bitmap= Bitmap.CreateBitmap(COLORDRAWABLE_DIMENSION, COLORDRAWABLE_DIMENSION, BITMAP_CONFIG);
				} else {
					bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, BITMAP_CONFIG);
				}

				Canvas canvas = new Canvas(bitmap);
				drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
				drawable.Draw(canvas);
				return bitmap;

			} catch (OutOfMemoryException e) {
				return null;
			}
		}
			
		private void Setup () 
		{
			if (!mReady) {
				mSetupPending = true;
				return;
			}

			if (mBitmap == null) {
				return;
			}

			mBitmapShader = new BitmapShader (mBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

			mBitmapPaint.AntiAlias = true;
			mBitmapPaint.SetShader(mBitmapShader);

			mBorderPaint.SetStyle (Paint.Style.Stroke);
			mBorderPaint.AntiAlias = true;
			mBorderPaint.Color = mBorderColor;
			mBorderPaint.StrokeWidth = mBorderWidth;

			mBitmapHeight = mBitmap.Height;
			mBitmapWidth = mBitmap.Width;

			mBorderRect.Set (0, 0, Width, Height);
			mBorderRadius = Math.Min ((mBorderRect.Height() - (float) mBorderWidth) / 2, (mBorderRect.Width() - (float)mBorderWidth) / 2);

			mDrawableRect.Set (mBorderWidth, mBorderWidth, mBorderRect.Width (), mBorderRect.Height () - mBorderWidth);
			mDrawableRadius = Math.Min(mDrawableRect.Height() / 2, mDrawableRect.Width() / 2);

			UpdateShaderMatrix();
			Invalidate();
		}

		private void UpdateShaderMatrix () 
		{
			float scale;
			float dx = 0;
			float dy = 0;

			mShaderMatrix.Set (null);

			if (mBitmapWidth * mDrawableRect.Height () > mDrawableRect.Width () * mBitmapHeight) {
				scale = mDrawableRect.Height() / (float)mBitmapHeight;
				dx = (mDrawableRect.Width () - mBitmapWidth * scale) * 0.5f;
			} else {
				scale = mDrawableRect.Width () / (float)mBitmapWidth;
				dy = (mDrawableRect.Height () - mBitmapHeight * scale) * 0.5f;
			}

			mShaderMatrix.SetScale (scale, scale);
			mShaderMatrix.PostTranslate ((int)(dx + 0.5f) + mBorderWidth, (int)(dy + 0.5f) + mBorderWidth);

			mBitmapShader.SetLocalMatrix (mShaderMatrix);
		}

	}
}

