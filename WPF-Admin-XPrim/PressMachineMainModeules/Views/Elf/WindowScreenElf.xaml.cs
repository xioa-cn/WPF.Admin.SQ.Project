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
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace PressMachineMainModeules.Views.Elf
{
    /// <summary>
    /// WindowScreenElf.xaml 的交互逻辑
    /// </summary>
    public partial class WindowScreenElf : Window
    {
        public bool IsShow { get; set; } = true;

        public WindowScreenElf()
        {
            InitializeComponent();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
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
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            OperationPopup.IsOpen = true;
        }
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            // 添加一个延迟，让用户有时间移动到 Popup 上
            Task.Delay(100).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    // 检查鼠标是否在 Popup 上
                    Point mousePosition = Mouse.GetPosition(OperationPopup);
                    if (!OperationPopup.IsMouseOver)
                    {
                        OperationPopup.IsOpen = false;
                    }
                });
            });
        }

        private void UniformGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void UniformGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
