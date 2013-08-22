using System;
using MonoTouch.Foundation;
using System.Globalization;

namespace ShinobiCharts.MvvmCrossBinding
{
	public static class NSObjectConversionExtensions
	{
		/// <summary>
		/// Attempts to convert native .net types to objC types.
		/// Works for:
		///  - DateTime
		///  - String
		///  - Number types
		/// </summary>
		/// <returns>The objective C representation</returns>
		/// <param name="o">The .net object</param>
		public static NSObject ConvertToNSObject(this object o)
		{
			NSObject toReturn;
			// Specific types first - DateTime
			if (o is DateTime) {
				toReturn = (NSDate)((DateTime)o);
			}
			// Now a String
			else if (o is string) {
				toReturn = (NSString)((string)o);
			}
			// And a catch-all
			else {
				toReturn = NSObject.FromObject (o);
			}

			// Send the result back
			return toReturn;
		}
	}
}

