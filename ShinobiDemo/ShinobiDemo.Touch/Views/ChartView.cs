using System;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using System.Drawing;
using ShinobiCharts;
using ShinobiCharts.MvvmCrossBinding;
using ShinobiDemo.Core.ViewModels;

namespace ShinobiDemo.Touch
{
	[Register("ChartView")]
	public class ChartView : MvxViewController, IMvxModalTouchView
	{
		private BindableDataSourceHelper<ExampleDataClass> _dsHelper;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Create a chart
			var chart = new ShinobiChart (new RectangleF (0, 40, View.Bounds.Width, View.Bounds.Height-40),
			                                   SChartAxisType.Number, SChartAxisType.Number);
			chart.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			// Create a datasource helper
			_dsHelper = new BindableDataSourceHelper<ExampleDataClass> (chart,
			                                                            () => { return new SChartLineSeries (); },
			"Time", "Lower"); 
			Add (chart);

			// Create a UISlider
			var slider = new UISlider (new RectangleF (0, 0, View.Bounds.Width, 40));
			slider.MinValue = 0;
			slider.MaxValue = 5;
			slider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			Add (slider);

			// Create the binding 
			var set = this.CreateBindingSet<ChartView, ChartViewModel> ();
			// Bind the datasource helper to the view model
			set.Bind (_dsHelper).For (s => s.Data).To (vm => vm.Source).OneWay();
			// And the UISlider
			set.Bind (slider).To (vm => vm.Frequency);
			set.Apply ();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Landscape;
		}
	}
}

