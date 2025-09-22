using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using OxyPlot;
using OxyPlot.Axes;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using PressMachineMainModeules.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using PressMachineMainModeules.Config;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// AutoEnvelopeLineWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AutoEnvelopeLineWindow : System.Windows.Window
    {
        public AutoEnvelopeLineWindow()
        {
            this.Loaded += AutoEnvelopeLineWindow_Loaded;
            this.Unloaded += AutoEnvelopeLineWindow_Unloaded;
            InitializeComponent();
        }

        private void AutoEnvelopeLineWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.Dialog.Unregister(
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogAutoEnvelopeLineToken, this);
            if(this.PlotModel.Model != null)
            {
                this.PlotModel.Model.MouseDown -= PlotModel_MouseDown;
            }
            
        }

        private void AutoEnvelopeLineWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HandyControl.Controls.Dialog.Register(
                WPF.Admin.Models.Models.HcDialogMessageToken.DialogAutoEnvelopeLineToken, this);
            this.PlotModel.Model.MouseDown += PlotModel_MouseDown;
        }

        private void DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        private void PlotModel_MouseDown(object? sender, OxyMouseDownEventArgs e)
        {
            // 获取图表的坐标轴（确保轴的Key与定义一致）
            var horizontalAxis =this.PlotModel.Model .GetAxis("HorizontalAxis") as LinearAxis; // 水平轴（X轴）
            var verticalAxis = this.PlotModel.Model.GetAxis("VerticalAxis") as LinearAxis;     // 垂直轴（Y轴）

            if (horizontalAxis == null || verticalAxis == null)
            {               
                return;
            }

            // 获取鼠标点击的屏幕坐标（相对于图表控件）
            // e.Position已经是相对于图表的坐标（X为水平方向，Y为垂直方向）
            OxyPlot.ScreenPoint screenPoint = e.Position;

            //  关键：将屏幕坐标转换为数据坐标
            // InverseTransform方法：将屏幕像素坐标转换为轴上的实际数据值
            double dataX = horizontalAxis.InverseTransform(screenPoint.X);
            double dataY = verticalAxis.InverseTransform(screenPoint.Y);

            //  输出转换后的坐标
            Debug.WriteLine($"数据坐标：X={dataX:F2}, Y={dataY:F2}");
            if (this.DataContext is  AutoEnvelopeLineViewModel viewModel)
            {
                var result = PointHelper.FindClosestPointIndex(viewModel.Posints, new System.Windows.Point(dataX, dataY));

                if (result == -1) return;
                ScrollerGo(result);
            }
        }
        private void ScrollerGo(int index)
        {
            scrollvirewer.ScrollToVerticalOffset(165 * index);
        }

        private void DelValue(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn)
            {
                if (btn.Tag is PosintModel model)
                {
                    if (this.DataContext is AutoEnvelopeLineViewModel viewModel)
                    {
                        viewModel.RemoveValue(model);
                    }
                }
            }
        }

        private void OrderByValue(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.PlotModel.Model != null)
            {
                this.PlotModel.Model.MouseDown -= PlotModel_MouseDown;
            }
            this.PlotModel.Model = null;
        }

        private void SaveData(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AutoEnvelopeLineViewModel viewModel)
            {
                WeakReferenceMessenger.Default.Send(new WeakEnvelopePosintView_Model()
                {
                    Points = viewModel.Posints.ToArray()
                });
                this.Close();
                this.DataContext = null;
                return;
            }

            Growl.ErrorGlobal(Common.t("AppErrorNotFoundVM"));
        }
    }
}
