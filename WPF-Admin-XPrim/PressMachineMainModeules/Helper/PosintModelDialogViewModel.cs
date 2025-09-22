using CommunityToolkit.Mvvm.ComponentModel;
using PressMachineMainModeules.Models;
using System.Windows;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.Helper
{
    public partial class PosintModelDialogViewModel : AdminDialogBase
    {
        [ObservableProperty] private string _title;
        [ObservableProperty] private PosintModel posintModel = new PosintModel();


        public PosintModelDialogViewModel(string title, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消",
            string yes = "是", string no = "否") : base(dialogToken, buttontype, ok, cancel, yes, no)
        {
            this.Title = title;
        }
    }
}
