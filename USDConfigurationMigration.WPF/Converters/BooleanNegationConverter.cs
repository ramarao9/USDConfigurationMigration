using System;
using System.Windows.Data;

namespace USDConfigurationMigration.WPF.Converters
{
    public class BooleanNegationConverter : IValueConverter
    {


        public BooleanNegationConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(value is bool && (bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(value is bool && (bool)value);
        }
    }
}
