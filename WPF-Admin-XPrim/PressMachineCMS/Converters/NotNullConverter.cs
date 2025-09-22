using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PressMachineCMS.Converters {
    /// <summary>
    /// 判断对象是否不为空的转换器
    /// </summary>
    public class NotNullConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}