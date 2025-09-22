using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;

namespace SystemModules.Converter
{
    internal class AuthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LoginAuth auth)
            {
                return auth switch
                {
                    LoginAuth.Admin => SystemModules.t("Admin"),
                    LoginAuth.Engineer => SystemModules.t("Engineer"),
                    LoginAuth.Employee => SystemModules.t("Employee"),
                    LoginAuth.FUser => "前台用户",
                    LoginAuth.HUser => "后台用户",
                    _ => "无权限"
                };
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}