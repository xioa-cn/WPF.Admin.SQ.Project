using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PressMachineCMS.Models;

namespace PressMachineCMS.Converters {
    public class VisualContentConverter:IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is VisualContentContent cc)
            {
                return cc.ToString();
            }
            return DependencyProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}