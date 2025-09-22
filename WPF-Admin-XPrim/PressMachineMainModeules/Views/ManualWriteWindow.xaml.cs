using System.Windows;

namespace PressMachineMainModeules.Views
{
    /// <summary>
    /// ManualWriteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ManualWriteWindow : Window
    {
        public ManualWriteWindow()
        {
            InitializeComponent();
            this.Closing += ManualWriteWindow_Closing;
        }

        private void ManualWriteWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // 取消关闭操作
            e.Cancel = true;
            // 改为隐藏窗口
            this.Hide();
        }
    }
}
