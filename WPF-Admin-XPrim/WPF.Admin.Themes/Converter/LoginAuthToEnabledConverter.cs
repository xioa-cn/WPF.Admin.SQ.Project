using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Converter;

public class LoginAuthToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (LoginAuthHelper.ViewAuthSwitch != ViewAuthSwitch.IsEnabled)
            return true;

        if (value is LoginAuth requiredAuth && LoginAuthHelper.LoginUser != null)
        {
            //return (int)LoginAuthHelper.LoginUser.LoginAuth >= (int)requiredAuth;

            if (requiredAuth == LoginAuth.None) return true;

            if (requiredAuth == LoginAuth.Admin &&
                LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Admin)
                return true;

            if (requiredAuth == LoginAuth.Engineer)
            {
                if (LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Admin ||
                    LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Engineer)
                    return true;
            }

            if (requiredAuth == LoginAuth.Employee)
            {
                if (LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Admin ||
                    LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Engineer ||
                    LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Employee)
                    return true;
            }

            return false;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}