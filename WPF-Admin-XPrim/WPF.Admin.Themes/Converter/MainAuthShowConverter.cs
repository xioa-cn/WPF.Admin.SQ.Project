using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Converter
{
    public class MainAuthShowConverter : IValueConverter
    {
        public LoginAuth LoginAuthProperty { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is LoginAuth la)
            {
                return la == LoginAuthProperty ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
