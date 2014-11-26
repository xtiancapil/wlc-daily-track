using System;
using ReactiveUI;
using System.Reactive.Linq;


namespace WlcDailyTracker.Core
{
	public class MainViewModel : ReactiveObject
	{
		int nutrition = 5;// = new int[5] { 0, 1, 2, 3, 4, 5 };
		bool workout = true;
		bool mobilize = true;
		bool supplement = true;
		bool lifestyle = true;
		bool water = true;
		bool reflectionWritten = false;
		string reflection = "";
		int bonus;
		public int earnedPoints = 5;

		public int Nutrition {
			get { return nutrition; }
			set { this.RaiseAndSetIfChanged (ref nutrition, value); }
		}

		public bool Workout {
			get { return workout; }
			set { this.RaiseAndSetIfChanged (ref workout, value); }
		}

		public bool Mobilize {
			get { return mobilize; }
			set { this.RaiseAndSetIfChanged (ref mobilize, value); }
		}

		public bool Supplement {
			get { return supplement; }
			set { this.RaiseAndSetIfChanged (ref supplement, value); }
		}

		public bool Lifestyle {
			get { return lifestyle; }
			set { this.RaiseAndSetIfChanged (ref lifestyle, value); }
		}

		public bool Water {
			get { return water; }
			set { this.RaiseAndSetIfChanged (ref water, value); }
		}

		public int Points {
			get { return earnedPoints; }
			set { this.RaiseAndSetIfChanged (ref earnedPoints, value); }
		}

		// TODO: bonus points are two - need logic check if entered 5 consecutive days.

		public MainViewModel()
		{
			this.WhenAny (x => x.Workout, x => x.Mobilize, x => x.Supplement, x => x.Lifestyle, x => x.Nutrition, x => x.Water,
				(workout, mobilize, supplement, lifestyle, nutrition, water) => (workout.Value? 2:0) + (mobilize.Value? 2:0) + (supplement.Value? 1:0) + (lifestyle.Value? 1:0) + nutrition.Value + (water.Value? 1:0)
			).Subscribe( x => { Points = x; });				
		}
	}
}

