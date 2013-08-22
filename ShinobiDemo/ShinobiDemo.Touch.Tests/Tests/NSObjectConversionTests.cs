using System;
using NUnit.Framework;
using MonoTouch.Foundation;
using ShinobiCharts.MvvmCrossBinding;


namespace ShinobiDemo.Touch.Tests
{
	[TestFixture]
	public class NSObjectConversionTests
	{
		[Test]
		public void NumberTypesConvertCorrectly ()
		{
			// Create all the different number types we can find
			byte n_byte = 12;
			sbyte n_sbyte = 13;
			int n_int = 14;
			uint n_uint = 15;
			short n_short = 16;
			ushort n_ushort = 17;
			long n_long = 18;
			ulong n_ulong = 19;
			float n_float = 20.0f;
			double n_double = 21.0;
			bool n_bool = true;
			char n_char = 'a';

			// Run their conversions
			TestTypeConversion<NSNumber> (n_byte.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_sbyte.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_int.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_uint.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_short.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_ushort.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_long.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_ulong.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_float.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_double.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_bool.ConvertToNSObject ());
			TestTypeConversion<NSNumber> (n_char.ConvertToNSObject ());
		}

		[Test]
		public void StringTypesConvertCorrectly()
		{
			string n_string = "Test String";
			TestTypeConversion<NSString> (n_string.ConvertToNSObject ());
			Assert.AreEqual(((NSString)n_string.ConvertToNSObject ()).ToString(), "Test String");
		}

		[Test]
		public void TimeTypesConvertCorrectly()
		{
			DateTime n_datetime = DateTime.Now;
			TestTypeConversion<NSDate> (n_datetime.ConvertToNSObject ());
		}


		private void TestTypeConversion<T>(NSObject o) {
			Assert.NotNull (o);
			Assert.True (o is T);
		}
	}
}
