using System;
using System.Windows.Data;

namespace SFLibs.UI.Converter
{
	[ValueConversion( typeof( string ), typeof( bool ) )]
	public class StringEmptyToBoolConverter : IValueConverter
	{
		public bool IsEmpty { get; set; }

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			var v = value as string;

			return string.IsNullOrEmpty( v ) ? this.IsEmpty : !this.IsEmpty;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
