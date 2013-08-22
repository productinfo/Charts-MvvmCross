using System;
using ShinobiCharts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;

namespace ShinobiCharts.MvvmCrossBinding
{
	public class BindableDataSourceHelper
	{
		private class ChartDataSource : SChartDataSource
		{
			private IList<SChartDataPoint> _dataPoints;

			public ChartDataSource (IEnumerable<Object> data, Func<object, NSObject> xValueConvertor,
			                        Func<object, NSObject> yValueConvertor)
			{
				if (data != null) {
					_dataPoints = data.Select (o => {
						return new SChartDataPoint () {
							XValue = xValueConvertor(o),
							YValue = yValueConvertor(o)
						};
					}).ToList ();
				} else {
					_dataPoints = new List<SChartDataPoint> ();
				}
			}

			#region implemented abstract members of SChartDataSource

			public override int GetNumberOfSeries (ShinobiChart chart)
			{
				return 1;
			}

			public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
			{
				return new SChartLineSeries ();
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

		private ShinobiChart _chart;
		private IEnumerable<object> _data;
		private ChartDataSource _dataSource;
		private string _xValueKey;
		private string _yValueKey;

		public BindableDataSourceHelper (ShinobiChart chart, string xValueKey, string yValueKey)
		{
			_chart = chart;
			_xValueKey = xValueKey;
			_yValueKey = yValueKey;
		}

		public IEnumerable<object> Data {
			get { return _data; }
			set {
				_data = value;
				DataUpdated ();
			}
		}

		private void DataUpdated ()
		{
			_dataSource = new ChartDataSource (_data, o => {
				var value = o.GetPropertyValue(_xValueKey);
				return value.ConvertToNSObject ();
			}, o => {
				var value = o.GetPropertyValue(_yValueKey);
				return value.ConvertToNSObject ();
			});
			_chart.DataSource = _dataSource;
			_chart.RedrawChart ();
		}
	}
}

