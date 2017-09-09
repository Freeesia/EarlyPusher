using System;
using System.Windows.Data;

namespace SFLibs.UI.Converter
{
	public class JoinMultiConverter : IMultiValueConverter
	{
		public IValueConverter Converter { get; set; }
		public IMultiValueConverter MultriConverter { get; set; }

		public object Convert( object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			object convertedValue = this.MultriConverter.Convert( values, targetType, parameter, culture );
			return this.Converter.Convert( convertedValue, targetType, parameter, culture );
		}

		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
