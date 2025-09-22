using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using PressMachineMainModeules.ViewModels;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;


namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// AutoPlotView.xaml 的交互逻辑
    /// </summary>
    public partial class AutoPlotView : System.Windows.Controls.UserControl
    {
        public AutoPlotView()
        {
            WeakReferenceMessenger.Default.Register<ThemeModel>(this, ChangedTheme);
            InitializeComponent();
            this.Loaded += AutoPlotView_Loaded;

        }

        private void AutoPlotView_Loaded(object sender, RoutedEventArgs e)
        {
            PlotColor();
        }

        private void ChangedTheme(object recipient, ThemeModel message)
        {
            PlotColor();
        }

        private void PlotColor()
        {
            var PlotModel = this.PlotModel.Model;
            if (PlotModel is not null)
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
                PlotModel.PlotAreaBorderColor = oxyBorderColor;  // 绘图区域边框颜色
                PlotModel.TextColor = oxyTextColor;              // 文本颜色
                PlotModel.PlotAreaBorderThickness = new OxyThickness(1);           // 边框粗细

                // 设置坐标轴
                foreach (var axis in PlotModel.Axes)
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


                PlotModel.InvalidatePlot(true);
            }
        }


        private double iscHeight = 0;
        private void ToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle)
            {
                var check = toggle.IsChecked;

                if (this.iscHeight == 0 || this.isc.ActualHeight != 0)
                    iscHeight = this.isc.ActualHeight;
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                if ((bool)check!)
                {
                    doubleAnimation.From = iscHeight;
                    doubleAnimation.To = 0;
                }
                else
                {
                    doubleAnimation.From = 0;
                    doubleAnimation.To = iscHeight;
                }

                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
                isc.BeginAnimation(System.Windows.Controls.UserControl.HeightProperty, doubleAnimation);
            }
        }


        private void ResultTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb && tb.Tag is AutoPlotValue plotValue 
                                                             && !string.IsNullOrEmpty(plotValue.Name)
                                                             && plotValue.Name.Contains("结果"))
            {
                if (tb.Text == "--")
                {
                    tb.Foreground = System.Windows.Media.Brushes.Purple;
                }
                else if (tb.Text == "Ok")
                {
                    tb.Foreground = System.Windows.Media.Brushes.Green;
                }
                else if (tb.Text == "Ng")
                {
                    tb.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    tb.Foreground = System.Windows.Media.Brushes.Black;
                }
            }
        }

        private void DoubleClick(object sender, MouseButtonEventArgs e) {
            if (this.DataContext is PlotUtils plotUtils)
            {
                plotUtils.Plot.ChangeParamOxyPlotView();
            }
        }
    }
}
