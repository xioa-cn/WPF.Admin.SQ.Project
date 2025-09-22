using System.Windows;
using System.Windows.Controls;

namespace WPF.Admin.Themes.W_Dialogs {
    public partial class TextBoxDialog : UserControl {
        private TextBoxDialogViewModel vm;
        public TextBoxDialog(string title, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否") {
            vm = new TextBoxDialogViewModel(title, dialogToken, buttontype, ok, cancel, yes, no);
            this.DataContext = vm;
            InitializeComponent();
        }
        
        public string GetDialogInputResult {
            get
            {
                return vm.InputText;
            }
        }
    }
}