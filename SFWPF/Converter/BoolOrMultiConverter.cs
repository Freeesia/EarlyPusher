using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SFLibs.UI.Converter
{
	[ValueConversion( typeof( bool ), typeof( bool ) )]
	public class BoolOrMultiConverter : IMultiValueConverter
	{
		public bool UnsetValue { get; set; }

		public object Convert( object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return values.Select( v => v == DependencyProperty.UnsetValue ? this.UnsetValue : v ).OfType<bool>().Any( v => v );
		}

		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
