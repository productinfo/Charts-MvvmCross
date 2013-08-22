using System;

namespace ShinobiCharts.MvvmCrossBinding
{
	public static class PropertyExtenstions
	{
		/// <summary>
		/// Gets the value of the named property.
		/// </summary>
		public static object GetPropertyValue(this object source, string propertyName)
		{
			var property = source.GetType().GetProperty(propertyName);
			if (property == null)
			{
				throw new ArgumentException(string.Format("The property {0} does not exist on the type {1}",
				                                          propertyName, source.GetType()));
			}
			return property.GetValue(source, null);
		}
	}
}

