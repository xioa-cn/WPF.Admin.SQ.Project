using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Admin.Themes.Converter;

public class MultiParamsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        if (value is object[] values)
        {
            return values;
        }
        return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
    }
}