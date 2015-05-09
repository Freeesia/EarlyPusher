using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EarlyPusher.Converters
{
	[ValueConversion( typeof( string ), typeof( Visibility ) )]
	public class RankVisibilityConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			var str = value as string;
			if( string.IsNullOrEmpty( str ) )
			{
				return Visibility.Hidden;
			}

			return Visibility.Visible;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
