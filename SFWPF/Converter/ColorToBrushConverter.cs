using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SFLibs.UI.Converter
{
	/// <summary>
	/// ColorをSlidColorBrushに変換するコンバーター
	/// </summary>
	[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
	public class ColorToBrushConverter : IValueConverter   
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return new SolidColorBrush(Colors.Transparent);
			}
			else
			{
				return new SolidColorBrush((Color)value);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				return ((SolidColorBrush)value).Color;
			}
		}
	}
}
