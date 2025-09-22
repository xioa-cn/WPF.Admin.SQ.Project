using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Converter
{
    public class LoginAuthToVisibilityAuthConverter : IValueConverter
    {
        public LoginAuth LoginAuth { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (LoginAuthHelper.ViewAuthSwitch != ViewAuthSwitch.Visibility)
                return Visibility.Visible;
            if (value is LoginAuth auth)
            {
                if ((int)auth <= (int)LoginAuth)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
