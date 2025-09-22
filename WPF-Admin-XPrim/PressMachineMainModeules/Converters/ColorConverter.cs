using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.ViewModels;

namespace PressMachineMainModeules.Converters;

public class ColorConverter :IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is BtnType btn)
        {
            return btn switch {
                BtnType.Info=>System.Windows.Media. Brushes.BlueViolet,
                BtnType.Success=>System.Windows.Media. Brushes.Green,
                BtnType.Error=>System.Windows.Media. Brushes.Red,
                BtnType.Warning=>System.Windows.Media. Brushes.Orange,
                _ =>System.Windows.Media. Brushes.Black
            };
        }
        return DependencyProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}