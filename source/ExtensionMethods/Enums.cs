using Mapbox.VectorTile.Geometry;
using System;
using System.ComponentModel;

namespace Mapbox.VectorTile.ExtensionMethods
{
	/// <summary>
	/// Extension method to extract the [Description] attribute from an Enum
	/// </summary>
	public static class EnumExtensions
	{
		public static string Description(this Enum value)
		{
			var enumType = value.GetType();
#if PORTABLE || WINDOWS_UWP
			var field = enumType.GetRuntimeField(value.ToString());
            object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length == 0 ? value.ToString() : ((DescriptionAttribute)attributes.First()).Description;
#else
			var field = enumType.GetField(value.ToString());
			var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length == 0 ? value.ToString() : ((DescriptionAttribute)attributes[0]).Description;
#endif
		}
	}
}
