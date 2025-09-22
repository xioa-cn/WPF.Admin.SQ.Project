using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PressMachineMainModeules.Components;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Converters {
    public class AutoParameterViewConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is not AutoParameterContentModel mo)
            {
                return DependencyProperty.UnsetValue;
            }

            if (mo.Type.ToLower() == "bool")
            {
                return new AutoParametersBooleanValue() { AutoParameterContent = mo };
            }
            else
            {
                return new AutoParametersNumberValue() { AutoParameterContent = mo };
            }

        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}