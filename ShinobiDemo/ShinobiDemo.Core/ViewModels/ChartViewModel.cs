using System;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;

namespace ShinobiDemo.Core.ViewModels
{
	/// <summary>
	/// View model for our harmonics chart
	/// </summary>
	public class ChartViewModel
		: MvxViewModel
	{
		/// <summary>
		/// The source data collection. This is bindable.
		/// </summary>
		private IList<ExampleDataClass> _source;
		public IList<ExampleDataClass> Source {
			get { return _source; }
			set {
				_source = value;
				RaisePropertyChanged (() => Source);
			}
		}

		/// <summary>
		/// The frequency of our sine wave
		/// </summary>
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

	/// <summary>
	/// A domain object. This will represent a data point.
	/// </summary>
	public class ExampleDataClass
	{
		public ExampleDataClass (double time, double lower, double upper)
		{
			this.Time = time;
			this.Lower = lower;
			this.Upper = upper;
		}

		public double Time { get; private set; }
		public double Lower { get; private set; }
		public double Upper { get; private set; }
	}
	 
}

