using System;
using System.Linq;
using ShinobiCharts;
using System.Collections.Generic;
using MonoTouch.Foundation;
using ShinobiDemo.Core.ViewModels;

namespace ShinobiDemo.Touch
{
	public class SChartDataSourceBinding : SChartDataSource
	{
		private IList<IList<SChartDataPoint>> _datapoints;
		private IList<SChartSeries> _series;
		private ShinobiChart _chart;

		/// <summary>
		/// The data property which is used for data binding
		/// </summary>
		private IEnumerable<IEnumerable<DataPoint>> _data;
		public IEnumerable<IEnumerable<DataPoint>> Data {
			get { return _data; }
			set {
				_data = value;
				UpdateSeries ();
				UpdateDataPoints ();
				// Tell the chart to reload its data and redraw
				_chart.ReloadData ();
				_chart.RedrawChart ();
			}
		}

		public SChartDataSourceBinding(ShinobiChart chart)
		{
			// Save the chart so we can reload it later on
			_chart = chart;
		}
		

		private void UpdateDataPoints()
		{
			if(_data != null) {
				_datapoints = new List<IList<SChartDataPoint>> ();
				foreach(var series in _data) {
					var newSeries = series
						.Select(dp => new SChartDataPoint()
						        { XValue = new NSNumber(dp.XValue),
								  YValue = new NSNumber(dp.YValue) })
							.ToList();
					_datapoints.Add (newSeries);
				}
			} else {
				_datapoints = null;
			}
		}

		private void UpdateSeries()
		{
			if (_data != null) {
				_series = new List<SChartSeries> ();
				foreach (var seriesData in _data) {
					_series.Add (new SChartLineSeries ());
				}
			} else {
				_series = null;
			}
		}


		#region implemented abstract members of SChartDataSource
		public override int GetNumberOfSeries (ShinobiChart chart)
		{
			if (_series != null) {
				return _series.Count;
			} else {
				return 1;
			}
		}

		public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
		{
			if (_series != null) {
				return _series [dataSeriesIndex];
			} else {
				return new SChartLineSeries();
			}
		}
		public override int GetNumberOfDataPoints (ShinobiChart chart, int dataSeriesIndex)
		{
			if (_data != null) {
				return _datapoints [dataSeriesIndex].Count;
			} else {
				return 0;
			}
		}
		public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
		{
			if (_data != null) {
				return _datapoints [dataSeriesIndex] [dataIndex];
			} else {
				return null;
			}
		}
		#endregion
	}
}

