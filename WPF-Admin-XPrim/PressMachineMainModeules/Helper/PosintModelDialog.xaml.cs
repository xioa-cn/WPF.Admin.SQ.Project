using PressMachineMainModeules.Models;
using System.Windows;

namespace PressMachineMainModeules.Helper
{
    /// <summary>
    /// PosintModelDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PosintModelDialog : System.Windows.Controls.UserControl
    {
        private PosintModelDialogViewModel vm;

        public PosintModelDialog(string title, string dialogToken,
            MessageBoxButton buttonType = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否")
        {
            vm = new PosintModelDialogViewModel(title, dialogToken, buttonType, ok, cancel, yes, no);
            this.DataContext = vm;
            InitializeComponent();
        }

        public PosintModel GetDialogPosintModelResult
        {
            get
            {
                return vm.PosintModel;
            }
        }
    }
}
