using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Converter
{
    public class LoginAuthToEnabledAuthConverter : IValueConverter
    {
        public LoginAuth LoginAuth { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (LoginAuthHelper.ViewAuthSwitch != ViewAuthSwitch.IsEnabled)
                return true;
            if (value is LoginAuth auth)
            {
                if ((int)auth <= (int)LoginAuth)
                {
                    return true;
                }
                return false;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
