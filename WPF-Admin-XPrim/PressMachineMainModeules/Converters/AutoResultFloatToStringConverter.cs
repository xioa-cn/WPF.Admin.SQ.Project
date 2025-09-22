using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PressMachineMainModeules.Converters
{
    public class AutoResultVisibilityConverter : IValueConverter
    {
        public string ValueName { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string svalue)
            {
                if(!svalue.Contains("结果") && ValueName == "Normal")
                {
                    return Visibility.Visible;
                }
                if(svalue.Contains("结果") && ValueName == "Normal")
                {
                    return Visibility.Collapsed;
                }
                if (svalue.Contains("结果") || ValueName == "Normal")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }


    public class AutoResultFloatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float fvalue)
                if (fvalue <= 0.1)
                {
                    return "--";
                }
                else if (fvalue <= 1.1)
                {
                    return "Ok";
                }
                else
                {
                    return "Ng";
                }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
