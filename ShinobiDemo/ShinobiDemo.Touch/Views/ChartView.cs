using System;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using System.Drawing;
using ShinobiCharts;

namespace ShinobiDemo.Touch
{
	[Register("ChartView")]
	public class ChartView : MvxViewController, IMvxModalTouchView
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var chart = new BoundShinobiChart (new RectangleF (0, 40, View.Bounds.Width, View.Bounds.Height-40),
			                                   SChartAxisType.Number, SChartAxisType.Number);
			chart.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			Add (chart);

			var slider = new UISlider (new RectangleF (0, 0, View.Bounds.Width, 40));
			slider.MinValue = 0;
			slider.MaxValue = 5;
			slider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			Add (slider);

			var set = this.CreateBindingSet<ChartView, ShinobiDemo.Core.ViewModels.ChartViewModel> ();
			set.Bind (chart).For (s => s.Data).To (vm => vm.Source).OneWay();
			set.Bind (slider).To (vm => vm.Frequency);
			set.Apply ();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Landscape;
		}
	}
}

