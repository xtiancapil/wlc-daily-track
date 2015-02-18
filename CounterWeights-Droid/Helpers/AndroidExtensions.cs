using System;

namespace CounterWeightsDroid
{
	public static class AndroidExtensions
	{
		public static bool IsMaterial {
			get {
				return Android.OS.Build.VERSION.SdkInt == Android.OS.BuildVersionCodes.Lollipop;
			}
		}
	}
}

