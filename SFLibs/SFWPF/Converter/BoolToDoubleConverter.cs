using System;
using System.Windows.Data;

namespace SFLibs.UI.Converter
{
	[ValueConversion( typeof( bool ), typeof( double ) )]
	public class BoolToDoubleConverter : IValueConverter
	{
		public double False { get; set; }
		public double True { get; set; }

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (bool)value ? this.True : this.False;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
