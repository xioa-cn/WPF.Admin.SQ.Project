using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Converters {
    public class AutoParameterDefaultConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is ObservableCollection<AutoParameterContentModel> autoParameterContents)
            {
                return autoParameterContents.Where(e => !e.Desc.Contains("#") );
            }

            return DependencyProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}