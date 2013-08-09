using System;
using ShinobiCharts;
using MonoTouch.Foundation;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;

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

			public BoundDataSource(IEnumerable<IEnumerable<ShinobiDemo.Core.ViewModels.DataPoint>> data)
			{
				_data = new List<IList<SChartDataPoint>> ();
				foreach(var series in data) {
					var seriesPoints = new List <SChartDataPoint>(series);
					_data.Add (seriesPoints);					
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
				return _data.Count;
			}

			public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
			{
				return _series [dataSeriesIndex];
			}
			public override int GetNumberOfDataPoints (ShinobiChart chart, int dataSeriesIndex)
			{
				return _data [dataSeriesIndex].Count;
			}
			public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
			{
				return _data [dataSeriesIndex] [dataIndex];
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
		}

		public BoundShinobiChart(RectangleF frame, SChartTheme theme)
			: base(frame, theme)
		{
		}
		#endregion

		#region Properties for binding
		private IEnumerable<IEnumerable<SChartDataPoint>> _data;
		public IEnumerable<IEnumerable<SChartDataPoint>> Data {
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
			this.ReloadData ();
		}
	}
}

