using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

#if WINDOWS_WPF
using System.Windows.Data;
using System.Windows.Markup;
#elif WINDOWS_UWP
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#elif XAMARIN_FORMS
using Xamarin.Forms;
#endif

namespace SFLibs.UI.Converter
{
#if WINDOWS_WPF || XAMARIN_FORMS
    [ContentProperty(nameof(Converters))]
#elif WINDOWS_UWP 
    [ContentProperty(Name = nameof(Converters))]
#endif
    public class JoinConverter : IValueConverter
    {

#if WINDOWS_WPF || WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
        public List<IValueConverter> Converters { get; private set; }

        public JoinConverter()
        {
            this.Converters = new List<IValueConverter>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return InnerConvert(value, targetType, parameter, culture);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return InnerConvert(value, targetType, parameter, language);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return InnerConvertBack(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return InnerConvertBack(value, targetType, parameter, language);
        }

        private object InnerConvert(object value, Type targetType, object parameter, dynamic culture)
        {
            object v = value;
            foreach (var conv in this.Converters)
            {
                v = conv.Convert(v, targetType, parameter, culture);
            }

            return v;
        }

        private object InnerConvertBack(object value, Type targetType, object parameter, dynamic culture)
        {
            object v = value;
            foreach (var conv in this.Converters)
            {
                v = conv.ConvertBack(v, targetType, parameter, culture);
            }

            return v;
        }
    }
}
