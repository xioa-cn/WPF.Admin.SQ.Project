using System.Windows;
using WPF.Admin.Models.Models;

namespace PressMachineMainModeules.Helper
{
    public partial class SignalDbModelDialog : System.Windows.Controls.UserControl
    {
        private SignalDbModelDialogViewModel vm;

        public SignalDbModelDialog(string title, string dialogToken,
            MessageBoxButton buttonType = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否")
        {
            vm = new SignalDbModelDialogViewModel(title, dialogToken, buttonType, ok, cancel, yes, no);
            this.DataContext = vm;
            InitializeComponent();
        }


        public SignalDbModel GetDialogSignalDbModelResult
        {
            get { return vm.SignalDbModel; }
        }
    }
}