using System;
using ShinobiCharts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;

namespace ShinobiCharts.MvvmCrossBinding
{

	/// <summary>
	/// A helper for a ShinobiChart datasource - which provides a Data
	/// property which can be data-bound.
	/// </summary>
	public class BindableDataSourceHelper<T>
	{
		private class ChartDataSource<Tds> : SChartDataSource
		{
			private IList<SChartDataPoint> _dataPoints;

			public SChartSeries Series { get; set; }

			public ChartDataSource (IList<Tds> data, SChartSeries series,
			                        Func<object, NSObject> xValueConvertor,
			                        Func<object, NSObject> yValueConvertor)
			{
				if (data != null) {
					// Convert the provided data objects to SChartDataPoints
					_dataPoints = data.Select (o => {
						return new SChartDataPoint () {
							XValue = xValueConvertor(o),
							YValue = yValueConvertor(o)
						};
					}).ToList ();
				} else {
					_dataPoints = new List<SChartDataPoint> ();
				}
				Series = series;
			}

			#region implemented abstract members of SChartDataSource
			public override int GetNumberOfSeries (ShinobiChart chart)
			{
				return 1;
			}

			public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
			{
				return Series;
			}

			public override int GetNumberOfDataPoints (ShinobiChart chart, int dataSeriesIndex)
			{
				return _dataPoints.Count;
			}

			public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
			{
				return _dataPoints [dataIndex];
			}

			#endregion
		}

		#region Member variables
		private ShinobiChart _chart;
		private ChartDataSource<T> _dataSource;
		private string _xValueKey;
		private string _yValueKey;
		#endregion

		#region Constructors
		public BindableDataSourceHelper (ShinobiChart chart, string xValueKey, string yValueKey)
			: this(chart, () => { return new SChartLineSeries(); }, xValueKey, yValueKey)
		{
		}
		/// <summary>
		/// Create new BindableDataSourceHelper
		/// </summary>
		/// <param name="chart">The ShinobiChart we wish to provide data to</param>
		/// <param name="seriesCreator">A lambda which will return the SChartSeries for this data</param>
		/// <param name="xValueKey">The name of the property in our domain objects to use as the x-value</param>
		/// <param name="yValueKey">The name of the property in our domain objects to use as the y-value</param>
		public BindableDataSourceHelper (ShinobiChart chart, Func<SChartSeries> seriesCreator, string xValueKey, string yValueKey)
		{
			_chart = chart;
			_xValueKey = xValueKey;
			_yValueKey = yValueKey;
			ChartSeries = seriesCreator;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The data to be plotted on the chart. Bindable.
		/// </summary>
		private IList<T> _data;
		public IList<T> Data {
			get { return _data; }
			set {
				_data = value;
				DataUpdated ();
			}
		}

		/// <summary>
		/// A lambda which will return a new SChartSeries object for this data
		/// </summary>
		private Func<SChartSeries> _chartSeriesCreator;
		public Func<SChartSeries> ChartSeries {
			get{ return _chartSeriesCreator; }
			set {
				_chartSeriesCreator = value;
				SeriesUpdated ();
			}
		}
		#endregion

		#region Private utility methods
		private void DataUpdated ()
		{
			// Replace the datasource with a new one
			_dataSource = new ChartDataSource<T> (_data, ChartSeries(),
			                                   o => {
				// Get the value for the specified property key and convert it to an NSObject
				return o.GetPropertyValue (_xValueKey).ConvertToNSObject ();
			},
			                                   o => {
				return o.GetPropertyValue (_yValueKey).ConvertToNSObject ();
			});
			_chart.DataSource = _dataSource;
			// Need to redraw to see the changes
			_chart.RedrawChart ();
		}

		private void SeriesUpdated ()
		{
			// If we don't have a datasource then we don't need to do anything
			if (_dataSource != null) {
				_dataSource.Series = ChartSeries();
				_chart.RedrawChart ();
			}
		}
		#endregion
	}
}

