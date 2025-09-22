using System.Windows;
using System.Windows.Controls;

namespace WPF.Admin.Themes.W_Dialogs {
    public partial class TextDialog : UserControl {
        public TextDialog(string message, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否") {
            this.DataContext = new TextDialogViewModel(message, dialogToken, buttontype, ok, cancel, yes, no);
            InitializeComponent();
        }
    }
}