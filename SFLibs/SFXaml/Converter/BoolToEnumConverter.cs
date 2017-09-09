using System;
using System.Globalization;

#if WINDOWS_WPF
using System.Windows.Data;
#elif WINDOWS_UWP
using Windows.UI.Xaml.Data;
#elif XAMARIN_FORMS
using Xamarin.Forms;
#endif

namespace SFLibs.UI.Converter
{
	public class BoolToEnumConverter : IValueConverter
	{
		public Type EnumType { get; set; }

		public int TrueValue { get; set; }

		public int FalseValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter, culture.TwoLetterISOLanguageName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var e = Enum.ToObject(this.EnumType, (bool)value ? this.TrueValue : this.FalseValue);
			return e;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
