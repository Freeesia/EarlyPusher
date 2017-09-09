using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SFLibs.UI.Converter
{
	[ValueConversion( typeof( int ), typeof( bool ) )]
	public class IntToBoolConverter : IValueConverter
	{
		public int Threshold { get; set; }

		public ComparisonOperator Operater { get; set; }

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if( value is int )
			{
				var v = (int)value;
				switch( this.Operater )
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

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
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
