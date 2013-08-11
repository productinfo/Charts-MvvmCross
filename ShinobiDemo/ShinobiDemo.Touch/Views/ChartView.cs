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
	public class ChartView : MvxViewController
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var chart = new BoundShinobiChart (View.Bounds, SChartAxisType.Number, SChartAxisType.Number);
			Add (chart);

			var slider = new UISlider (new RectangleF (10, 120, 300, 40));
			slider.MinValue = 0;
			slider.MaxValue = 5;
			Add (slider);

			var set = this.CreateBindingSet<ChartView, ShinobiDemo.Core.ViewModels.ChartViewModel> ();
			set.Bind (chart).For (s => s.Data).To (vm => vm.Source).OneWay();
			set.Bind (slider).To (vm => vm.Frequency);
			set.Apply ();
		}
	}
}

