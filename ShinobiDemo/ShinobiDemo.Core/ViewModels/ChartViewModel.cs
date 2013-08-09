using System;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;
using ShinobiCharts;

namespace ShinobiDemo.Core.ViewModels
{
	public class ChartViewModel
		: MvxViewModel
	{
		private IEnumerable<IEnumerable<DataPoint>> _source;
		public IEnumerable<IEnumerable<DataPoint>> Source {
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
			var dps = new List<List<DataPoint>>();
			// create the upper and lower component
			var upperHarmonic = new List<DataPoint>();
			var lowerHarmonic = new List<DataPoint>();
			for (double phase = 0; phase < Math.PI; phase+= (Math.PI / 100))
			{
				upperHarmonic.Add(new DataPoint(phase, Math.Sin(phase * this.Frequency) + this.Frequency * 2.5));
				lowerHarmonic.Add(new DataPoint(phase, Math.Sin(phase * this.Frequency + Math.PI) + this.Frequency * 2.5));
			}

			// add each to the collection
			dps.Add(upperHarmonic);
			dps.Add(lowerHarmonic);

			// And then save this off - making sure to use the setter
			this.Source = dps;
		}
	}


	public class DataPoint
	{
		private double _xValue;
		public double XValue {
			get { return _xValue;}
			set { _xValue = value;}
		}

		private double _yValue;
		public double YValue {
			get { return _yValue;}
			set { _yValue = value;}
		}
	}
	 
}

