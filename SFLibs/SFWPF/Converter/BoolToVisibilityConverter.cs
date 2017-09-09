using System;
using System.Windows;
using System.Windows.Data;

namespace SFLibs.UI.Converter
{
	[ValueConversion( typeof( bool ), typeof( Visibility ) )]
	public class BoolToVisibilityConverter : IValueConverter
	{
		public Visibility False { get; set; }
		public Visibility True { get; set; }

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return ( bool )value ? this.True : this.False;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			var vis = ( Visibility )value;
			if( vis == this.False )
			{
				return false;
			}
			else if( vis == this.True )
			{
				return true;
			}

			return null;
		}
	}
}
