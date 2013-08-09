using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore.IoC;

namespace ShinobiDemo.Core
{
	public class App : MvxApplication
	{
		public override void Initialize ()
		{
			CreatableTypes ()
				.EndingWith ("Service")
				.AsInterfaces ()
				.RegisterAsLazySingleton ();

			RegisterAppStart<ViewModels.ChartViewModel> ();
		}
	}
}

