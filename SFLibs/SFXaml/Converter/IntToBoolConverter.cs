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
    public class IntToBoolConverter : IValueConverter
    {
        public int Threshold { get; set; }

        public ComparisonOperator Operater { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture.TwoLetterISOLanguageName);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int v)
            {
                switch (this.Operater)
                {
                    case ComparisonOperator.Equal:
                        return this.Threshold == v;
                    case ComparisonOperator.Unequal:
                        return this.Threshold != v;
                    case ComparisonOperator.LessThan:
                        return this.Threshold > v;
                    case ComparisonOperator.LessThanOrEqualTo:
                        return this.Threshold >= v;
                    case ComparisonOperator.GreaterThan:
                        return this.Threshold < v;
                    case ComparisonOperator.GreaterThanOrEqualTo:
                        return this.Threshold <= v;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public enum ComparisonOperator
    {
        Equal,
        Unequal,
        LessThan,
        LessThanOrEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo
    }
}
