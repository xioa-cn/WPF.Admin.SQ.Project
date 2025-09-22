using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PressMachineMainModeules.Config;

namespace PressMachineMainModeules.Converters;

public class YPointTestConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is bool b)
        {
            return b ? Common.t("Trigger.Trigger") : Common.t("Trigger.Stop");
        }
        
        return DependencyProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}