using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF.Admin.Themes.W_Dialogs {
    public partial class TextBoxDialogViewModel : AdminDialogBase {
        [ObservableProperty] private string _title;

        [ObservableProperty] private string _inputText = string.Empty;

        public TextBoxDialogViewModel(string title, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消", string yes = "是", string no = "否") : base(dialogToken, buttontype,
            ok, cancel, yes, no) {
            this.Title = title;
        }
    }
}