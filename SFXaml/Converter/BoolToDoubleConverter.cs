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
    public class BoolToDoubleConverter : IValueConverter
    {
        public double TrueValue { get; set; }

        public double FalseValue { get; set; }

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
            return (bool)value ? this.TrueValue : this.FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
