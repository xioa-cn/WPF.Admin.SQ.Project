using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;


namespace WPF.Admin.Themes.Converter;

public class LoginAuthToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (LoginAuthHelper.ViewAuthSwitch != ViewAuthSwitch.Visibility)
            return Visibility.Visible;

        if (value is LoginAuth requiredAuth && LoginAuthHelper.LoginUser != null)
        {
            if (requiredAuth == LoginAuth.None) return Visibility.Visible;

            if (requiredAuth == LoginAuth.Admin &&
                LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Admin)
                return Visibility.Visible;

            if (requiredAuth == LoginAuth.Engineer)
            {
                if (LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Admin ||
                    LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Engineer)
                    return Visibility.Visible;
            }

            if (requiredAuth == LoginAuth.Employee)
            {
                if (LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Admin ||
                    LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Engineer ||
                    LoginAuthHelper.LoginUser.LoginAuth == LoginAuth.Employee)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}