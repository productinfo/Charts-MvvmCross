using System;
using ShinobiCharts;
using MonoTouch.Foundation;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using ShinobiDemo.Core.ViewModels;

namespace ShinobiDemo.Touch
{
	[Register("BoundShinobiChart")]
	public class BoundShinobiChart : ShinobiChart
	{
		#region Auxiliary classes
		private class BoundDataSource : SChartDataSource
		{
			private IList<IList<SChartDataPoint>> _data;
			private IList<SChartSeries> _series;

			public BoundDataSource(IntPtr h) : base(h)
			{
			}

			public BoundDataSource(IEnumerable<IEnumerable<DataPoint>> data)
			{
				if(data != null) {
					_data = new List<IList<SChartDataPoint>> ();
					foreach(var series in data) {
						var newSeries = series
							.Select(dp => new SChartDataPoint()
							        { XValue = new NSNumber(dp.XValue),
									  YValue = new NSNumber(dp.YValue) })
							.ToList();
						_data.Add (newSeries);
						UpdateSeries();
					}
				} else {
					_data = null;
					_series = null;
				}
			}

			private void UpdateSeries()
			{
				_series = new List<SChartSeries> ();
				foreach (var seriesData in _data) {
					_series.Add (new SChartLineSeries());
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
					return _data [dataSeriesIndex].Count;
				} else {
					return 0;
				}
			}
			public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
			{
				if (_data != null) {
					return _data [dataSeriesIndex] [dataIndex];
				} else {
					return null;
				}
			}
			#endregion
		}
		#endregion

		#region Member variables
		private BoundDataSource _boundDataSource;
		#endregion

		#region Constructors
		public BoundShinobiChart (IntPtr h)
			: base(h)
		{
		}

		public BoundShinobiChart(RectangleF frame)
			: base(frame)
		{
			UpdateData ();
		}

		public BoundShinobiChart(RectangleF frame, SChartTheme theme)
			: base(frame, theme)
		{
		}

		public BoundShinobiChart(RectangleF frame, SChartAxisType xAxisType, SChartAxisType yAxisType)
			: base(frame, xAxisType, yAxisType)
		{
			UpdateData();
		}

		#endregion

		#region Properties for binding
		private IEnumerable<IEnumerable<DataPoint>> _data;
		public IEnumerable<IEnumerable<DataPoint>> Data {
			get { return _data;}
			set {
				_data = value;
				UpdateData ();
			}
		}
		#endregion

		private void UpdateData()
		{
			_boundDataSource = new BoundDataSource (_data);
			this.DataSource = _boundDataSource;
			this.RedrawChart ();
		}
	}
}

