using System;
using ShinobiCharts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;

namespace ShinobiDemo.Touch
{
	public class BindableDataSourceHelper
	{
		private class ChartDataSource : SChartDataSource
		{
			private IList<SChartDataPoint> _dataPoints;

			public ChartDataSource (IList<Object> data, Func<object, NSObject> xValueConvertor,
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
				throw new NotImplementedException ();
			}

			public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
			{
				throw new NotImplementedException ();
			}

			public override int GetNumberOfDataPoints (ShinobiChart chart, int dataSeriesIndex)
			{
				throw new NotImplementedException ();
			}

			public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
			{
				throw new NotImplementedException ();
			}

			#endregion

		}

		private ShinobiChart _chart;
		private IList<object> _data;
		private ChartDataSource _dataSource;
		private string _xValueKey;
		private string _yValueKey;

		public BindableDataSourceHelper (ShinobiChart chart, string xValueKey, string yValueKey)
		{
			_chart = chart;
			_xValueKey = xValueKey;
			_yValueKey = yValueKey;
		}

		public IList<object> Data {
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
				return ConvertFromNetToObjC (value);
			}, o => {
				var value = o.GetPropertyValue(_yValueKey);
				return ConvertFromNetToObjC (value);
			});
			_chart.DataSource = _dataSource;
			_chart.RedrawChart ();
		}

		private NSObject ConvertFromNetToObjC(object o)
		{
			return (NSNumber)o;
		}


	}
}

