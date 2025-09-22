using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Converters {
    public class AutoParameterVisibilityDefaultConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is ObservableCollection<AutoParameterContentModel> autoParameterContents)
            {
                return autoParameterContents.Count(e => !e.Desc.Contains("#")) > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            return DependencyProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}