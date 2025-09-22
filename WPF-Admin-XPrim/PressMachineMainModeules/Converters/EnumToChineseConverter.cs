using PressMachineMainModeules.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using PressMachineMainModeules.Config;

namespace PressMachineMainModeules.Converters
{
    public class EnumToChineseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;



            if (value is MonitorType eMonitor)
            {
                switch (eMonitor)
                {
                    case MonitorType.BottomInRightOut:
                        return Common.t("MonitorType.BottomInRightOut");
                    case MonitorType.BottomInNoOut:
                        return Common.t("MonitorType.BottomInNoOut");
                    case MonitorType.BottomInBottomOut:
                        return Common.t("MonitorType.BottomInBottomOut");
                    case MonitorType.LeftInTopOut:
                        return Common.t("MonitorType.LeftInTopOut");
                    case MonitorType.LeftInRightOut:
                        return Common.t("MonitorType.LeftInRightOut");
                    case MonitorType.LeftInRightNoOut:
                        return Common.t("MonitorType.LeftInRightNoOut");
                }
            }

            return value; // 默认返回枚举的字符串表示
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
