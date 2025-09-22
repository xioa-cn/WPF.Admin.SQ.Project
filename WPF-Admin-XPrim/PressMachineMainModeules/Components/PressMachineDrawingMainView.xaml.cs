using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using PressMachineMainModeules.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF.Admin.Models.Models;
using Application = System.Windows.Application;


namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// PressMachineDrawingMainView.xaml 的交互逻辑
    /// </summary>
    public partial class PressMachineDrawingMainView : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty PloModelControlDtProperty =
            DependencyProperty.Register(
                "PloModelControlDt",
                typeof(PloModelControlDt), typeof(PressMachineDrawingMainView),
                new FrameworkPropertyMetadata(new PloModelControlDt(),
                    new PropertyChangedCallback(OnPloModelControlDtChanged)
                    )
                { BindsTwoWayByDefault = true }
                );

        private static void OnPloModelControlDtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineDrawingMainView)d).PloModelControlDt = (PloModelControlDt)e.NewValue!;
        }

        public static readonly DependencyProperty PlotModelProperty =
            DependencyProperty.Register(
                 "PlotModel",
                 typeof(PlotModel), typeof(PressMachineDrawingMainView),
                 new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnItemsSourceChanged)
                 )
                 { BindsTwoWayByDefault = true }
       );
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PressMachineDrawingMainView)d;
            if (e.NewValue is PlotModel model)
            {
                // 获取主题颜色
                var textColor = Application.Current.Resources["Text.Primary.Brush"] as SolidColorBrush;
                var borderColor = Application.Current.Resources["Border.Brush"] as SolidColorBrush;
                var gridColor = Application.Current.Resources["Border.Brush"] as SolidColorBrush;

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

                control.PlotModel = model;
                model.InvalidatePlot(true);
            }
        }

        public PlotModel? PlotModel
        {
            get { return (PlotModel)GetValue(PlotModelProperty); }
            set { SetValue(PlotModelProperty, value); }
        }



        public PloModelControlDt PloModelControlDt
        {
            get { return (PloModelControlDt)GetValue(PloModelControlDtProperty); }
            set { SetValue(PloModelControlDtProperty, value); }
        }

        public Visibility ShowText
        {
            get { return (Visibility)GetValue(ShowTextProperty); }
            set { SetValue(ShowTextProperty, value); }
        }
        public static readonly DependencyProperty ShowTextProperty =
           DependencyProperty.Register(
                "Visibility",
                typeof(Visibility), typeof(PressMachineDrawingMainView),
                new FrameworkPropertyMetadata(Visibility.Visible,
                   new PropertyChangedCallback(ShowTextOnMountedChanged)
                )
                { BindsTwoWayByDefault = true }
      );

        private static void ShowTextOnMountedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PressMachineDrawingMainView)d;
            control.ShowText = (Visibility)e.NewValue!;
        }

        public string PressMachineNo
        {
            get { return (string)GetValue(PressMachineNoProperty); }
            set { SetValue(PressMachineNoProperty, value); }
        }

        public static readonly DependencyProperty PressMachineNoProperty =
            DependencyProperty.Register(
                "PressMachineNo",
                typeof(string), typeof(PressMachineDrawingMainView),
                new FrameworkPropertyMetadata("未分配",
                    new PropertyChangedCallback(OnPressMachineNoChanged)
                    )
                { BindsTwoWayByDefault = true }
                );

        private static void OnPressMachineNoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineDrawingMainView)d).PressMachineNo = (string)e.NewValue!;
        }

        public PressMachineDrawingMainView()
        {
            this.Initialized += InitializedWeakManager;
            InitializeComponent();
            
            //this.Loaded += PressMachineDrawingMainView_Loaded;
            //this.Unloaded += PressMachineDrawingMainView_Unloaded;
        }

        private void InitializedWeakManager(object? sender, EventArgs e)
        {
            WeakReferenceMessenger.Default.Register<ThemeModel>(this, ChangedTheme);
        }

        private void PressMachineDrawingMainView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ChangedTheme(object recipient, ThemeModel message)
        {
            if (this.PlotModel is not null)
            {
                // 获取主题颜色
                var textColor = Application.Current.Resources["Text.Primary.Brush"] as SolidColorBrush;
                var borderColor = Application.Current.Resources["Border.Brush"] as SolidColorBrush;
                var gridColor = Application.Current.Resources["Border.Brush"] as SolidColorBrush;

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
                this.PlotModel.PlotAreaBorderColor = oxyBorderColor;  // 绘图区域边框颜色
                this.PlotModel.TextColor = oxyTextColor;              // 文本颜色
                this.PlotModel.PlotAreaBorderThickness = new OxyThickness(1);           // 边框粗细

                // 设置坐标轴
                foreach (var axis in this.PlotModel.Axes)
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

               
                this.PlotModel.InvalidatePlot(true);
            }
        }

        private void PressMachineDrawingMainView_Loaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<ThemeModel>(this);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PressMachineResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PressMachineResult.Text == "OK")
            {
                PressMachineResult.Foreground = System.Windows.Media.Brushes.Green;
            }
            else if (PressMachineResult.Text == "NG")
            {
                PressMachineResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void plotView_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 0.9 : 1.1;
            ShowCount *= zoomFactor;
        }


        public double ShowCount
        {
            get { return (double)GetValue(ShowCountProperty); }
            set { SetValue(ShowCountProperty, value); }
        }

        public static readonly DependencyProperty ShowCountProperty =
            DependencyProperty.Register(
                "ShowCount",
                typeof(double), typeof(PressMachineDrawingMainView),
                new FrameworkPropertyMetadata(0.0,
                    new PropertyChangedCallback(OnShowCountChanged)
                    )
                { BindsTwoWayByDefault = true }
                );

        private static void OnShowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PressMachineDrawingMainView)d).ShowCount = (double)e.NewValue!;
        }
    }
}
