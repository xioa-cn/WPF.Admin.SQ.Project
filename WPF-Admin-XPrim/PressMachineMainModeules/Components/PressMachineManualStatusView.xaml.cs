using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using PressMachineMainModeules.ViewModels;

namespace PressMachineMainModeules.Components
{
    /// <summary>
    /// PressMachineManualStatusView.xaml 的交互逻辑
    /// </summary>
    public partial class PressMachineManualStatusView : Window
    {
        public bool IsShow { get; set; }
        public PressMachineManualStatusView()
        {
            InitializeComponent();
            this.Closing += PressMachineManualStatusView_Closing;
        }

        private void PressMachineManualStatusView_Closing(object? sender, CancelEventArgs e)
        {
            // 取消关闭操作
            e.Cancel = true;
            // 改为隐藏窗口
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is PreManualViewModel vm)
            {
                vm.StatusWindowShow = false;
            }

            IsShow = false;
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
