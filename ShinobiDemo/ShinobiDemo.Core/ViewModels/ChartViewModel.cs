using System;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;

namespace ShinobiDemo.Core.ViewModels
{
	public class ChartViewModel
		: MvxViewModel
	{
		private IEnumerable<ExampleDataClass> _source;
		public IEnumerable<ExampleDataClass> Source {
			get { return _source; }
			set {
				_source = value;
				RaisePropertyChanged (() => Source);
			}
		}

		private double _frequency;
		public double Frequency {
			get { return _frequency;}
			set {
				_frequency = value;
				UpdateDataPoints ();
				RaisePropertyChanged (() => Frequency);
			}
		}

		private void UpdateDataPoints()
		{
			// create the upper and lower component
			var dps = new List<ExampleDataClass>();
			for (double phase = 0; phase < Math.PI; phase+= (Math.PI / 100))
			{
				dps.Add (new ExampleDataClass (phase,
				                               Math.Sin (phase * this.Frequency + Math.PI) + this.Frequency * 2.5,
				                               Math.Sin (phase * this.Frequency) + this.Frequency * 2.5));
			}

			// And then save this off - making sure to use the setter
			this.Source = dps;
		}
	}


	public class ExampleDataClass
	{
		public ExampleDataClass (double time, double lower, double upper)
		{
			this.Time = time;
			this.Lower = lower;
			this.Upper = upper;
		}

		private double _time;
		public double Time {
			get { return _time;}
			set { _time = value;}
		}

		private double _lower;
		public double Lower {
			get { return _lower;}
			set { _lower = value;}
		}

		private double _upper;
		public double Upper {
			get { return _upper;}
			set { _upper = value;}
		}
	}
	 
}

