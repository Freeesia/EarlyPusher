using System;
using System.Globalization;

#if WINDOWS_WPF
using System.Windows;
using System.Windows.Data;
#elif WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

#if !XAMARIN_FORMS
namespace SFLibs.UI.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture.TwoLetterISOLanguageName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture.TwoLetterISOLanguageName);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return b ? this.TrueValue : this.FalseValue;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility v)
            {
                if (v == this.TrueValue)
                {
                    return true;
                }
                else if (v == this.FalseValue)
                {
                    return false;
                }
            }

            return null;
        }
    }
}
#endif