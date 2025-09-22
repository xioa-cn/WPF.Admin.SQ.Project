using OxyPlot;
using System.Windows;
using System.Windows.Media;

namespace PressMachineMainModeules.Utils
{
    public partial class PlotThemesHelper
    {
        public static void SetThemePlot(PlotModel model)
        {
            // 获取主题颜色
            var textColor = System.Windows.Application.Current.Resources["Text.Primary.Brush"] as SolidColorBrush;
            var borderColor = System.Windows.Application.Current.Resources["Border.Brush"] as SolidColorBrush;
            var gridColor = System.Windows.Application.Current.Resources["Border.Brush"] as SolidColorBrush;

            // 转换为 OxyColor
            var oxyTextColor = OxyColor.FromArgb(
                textColor.Color.A,
                textColor.Color.R,
                textColor.Color.G,
                textColor.Color.B);

            var oxyBorderColor = OxyColor.FromArgb(
                borderColor.Color.A,
                borderColor.Color.R,
                borderColor.Color.G,
                borderColor.Color.B);

            // 设置 PlotModel 的整体外观
            model.PlotAreaBorderColor = oxyBorderColor;  // 绘图区域边框颜色
            model.TextColor = oxyTextColor;              // 文本颜色
            model.PlotAreaBorderThickness = new OxyThickness(1);           // 边框粗细

            // 设置坐标轴
            foreach (var axis in model.Axes)
            {
                axis.TextColor = oxyTextColor;           // 坐标轴文本颜色
                axis.TicklineColor = oxyBorderColor;     // 刻度线颜色
                axis.TitleColor = oxyTextColor;          // 坐标轴标题颜色
                axis.AxislineColor = oxyBorderColor;     // 坐标轴线颜色
                axis.MajorGridlineColor = OxyColor.FromAColor(40, oxyBorderColor); // 主网格线颜色
                axis.MinorGridlineColor = OxyColor.FromAColor(20, oxyBorderColor); // 次网格线颜色
                axis.AxislineThickness = 1;              // 坐标轴线粗细
                                                         // axis.th = 1;              // 刻度线粗细
                axis.MajorGridlineThickness = 1;         // 主网格线粗细
                axis.MinorGridlineThickness = 0.5;       // 次网格线粗细
                axis.MinorTickSize = 2;                  // 次刻度大小
                axis.MajorTickSize = 4;                  // 主刻度大小
            }
            model.InvalidatePlot(true);
        }
    }
}
