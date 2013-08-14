using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Views.Presenters;

namespace ShinobiDemo.Touch
{
	/// <summary>
	/// Hides the navigation bar of the simple view presenter.
	/// Allows simulation of a single-view application.
	/// </summary>

	public class SingleViewPresenter : MvxTouchViewPresenter
	{
		public SingleViewPresenter (UIApplicationDelegate appDelegate, UIWindow window)
			: base(appDelegate, window)
		{
		}

		protected override UINavigationController CreateNavigationController (UIViewController viewController)
		{
			var toReturn = base.CreateNavigationController (viewController);
			toReturn.NavigationBarHidden = true;
			return toReturn;
		}
	}
}
