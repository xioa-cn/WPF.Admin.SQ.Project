using System.Windows;
using System.Windows.Controls;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.W_Dialogs {
    public partial class KeyValueDialog : UserControl {
        private KeyValueDialogViewModel vm;

        public KeyValueDialog(string title, string dialogToken,
            MessageBoxButton buttonType = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否") {
            vm = new KeyValueDialogViewModel(title, dialogToken, buttonType, ok, cancel, yes, no);
            this.DataContext = vm;
            InitializeComponent();
        }

        public KeyValueDialogResult GetDialogKeyValueResult {
            get
            {
                return new KeyValueDialogResult() {
                    Key = vm.Key,
                    Value = vm.Value,
                };
            }
        }
    }
}