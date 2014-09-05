using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using WlcDailyTracker.Core;
using ReactiveUI;


namespace WlcDailyTrackAndroid
{
	[Activity (Label = "WlcDailyTrack.Android", Theme = "@style/WlcTheme")]
	public class MainActivity : ReactiveActivity<MainViewModel>
	{
		TextView pointsView;
		Spinner nutritionSpinner;
		Switch workoutSwitch;
		Switch supplementSwitch;
		Switch lifestyleSwitch;
		Switch mobilizeSwitch;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			ViewModel = new MainViewModel ();

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			pointsView = FindViewById<TextView> (Resource.Id.pointView);
			nutritionSpinner = FindViewById<Spinner> (Resource.Id.nutritionSpinner);
			workoutSwitch = FindViewById<Switch> (Resource.Id.workout_switch);
			supplementSwitch = FindViewById<Switch> (Resource.Id.supplement_switch);
			lifestyleSwitch = FindViewById<Switch> (Resource.Id.lifestyle_switch);
			mobilizeSwitch = FindViewById<Switch> (Resource.Id.mobilize_switch);

			var adapter = ArrayAdapter.CreateFromResource(this,
				Resource.Array.points_array, Android.Resource.Layout.SimpleSpinnerItem );

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			nutritionSpinner.Adapter = adapter;
			nutritionSpinner.ItemSelected += spinner_ItemSelected;
//			this.Bind (ViewModel, x => x.Workout , v => v.workoutSwitch.Checked, bool);
//			this.Bind (ViewModel, x => x.supplement , v => v.supplementSwitch.Checked);
//			this.Bind (ViewModel, x => x.lifestyle , v => v.lifestyleSwitch.Checked);
//			this.Bind (ViewModel, x => x.mobilize , v => v.mobilizeSwitch.Checked);			
			this.Bind (ViewModel, x => x.Points, v => v.pointsView.Text);

			workoutSwitch.CheckedChange += (sender, e) => ViewModel.Workout = e.IsChecked;
			supplementSwitch.CheckedChange += (sender, e) => ViewModel.Supplement = e.IsChecked;
			lifestyleSwitch.CheckedChange += (sender, e) => ViewModel.Lifestyle = e.IsChecked;
			mobilizeSwitch.CheckedChange += (sender, e) => ViewModel.Mobilize = e.IsChecked;
		}

		private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

			ViewModel.Nutrition = int.Parse( spinner.GetItemAtPosition (e.Position).ToString());
//
//			string toast = string.Format ("The planet is {0}", spinner.GetItemAtPosition (e.Position));
//			Toast.MakeText (this, toast, ToastLength.Long).Show ();
		}
	}
}


