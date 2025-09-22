using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PressMachineMainModeules.Converters
{
    public class ManualParameterI18nConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return "ManualParameter." + str;
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}