using System;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using System.Drawing;

namespace ShinobiDemo.Touch
{
	[Register("MyFirstView")]
	public class MyFirstView : MvxViewController
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var chart = new BoundShinobiChart (View.Bounds);
			Add (chart);

			var slider = new UISlider (new RectangleF (10, 120, 300, 40));
			slider.MinValue = 0;
			slider.MaxValue = 5;
			Add (slider);

			var set = this.CreateBindingSet<MyFirstView, ShinobiDemo.Core.ViewModels.ChartViewModel> ();
			set.Bind (chart).For (s => s.Data).To (vm => vm.Source);
			set.Bind (slider).To (vm => vm.Frequency).OneWay ();
			set.Apply ();
		}
	}
}

