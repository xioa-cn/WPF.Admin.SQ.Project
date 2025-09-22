using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.W_Dialogs;

namespace PressMachineMainModeules.Helper
{
    public partial class SignalDbModelDialogViewModel : AdminDialogBase
    {
        [ObservableProperty] private string _title;

        [ObservableProperty] private SignalDbModel _signalDbModel = new SignalDbModel();

        public SignalDbModelDialogViewModel(string title, string dialogToken,
            MessageBoxButton buttontype = MessageBoxButton.OK,
            string ok = "确认", string cancel = "取消",
            string yes = "是", string no = "否") : base(dialogToken, buttontype, ok, cancel, yes, no)
        {
            this.Title = title;
        }
        
        
       
    }
}