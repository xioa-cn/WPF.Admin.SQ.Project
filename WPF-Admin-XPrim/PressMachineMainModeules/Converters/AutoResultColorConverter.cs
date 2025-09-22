
using PressMachineMainModeules.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Converters
{
    public class AutoResultColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AutoPlotValue fvalue)
            {
                if (fvalue.Name.Contains("结果"))
                {
                    if (fvalue.Value <= 0.1)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                    else if (fvalue.Value <= 1.1)
                    {
                        return Brushes.Green;
                    }
                    else
                    {
                        return Brushes.Red;
                    }
                }
              
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
