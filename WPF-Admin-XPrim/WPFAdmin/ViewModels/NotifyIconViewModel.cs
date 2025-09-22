using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPFAdmin.Views;

namespace WPFAdmin.ViewModels;

public partial class NotifyIconViewModel : BindableBase, IDialogResultable<CloseEnum>
{
    public CloseEnum Result { get; set; }
    public Action CloseAction { get; set; }

    private Action<CloseEnum>? action;

    public NotifyIconViewModel(Action<CloseEnum>? action = null)
    {
        this.action = action;
    }

    [ObservableProperty] private bool _close = true;
    [ObservableProperty] private bool _mini;
    [ObservableProperty] private bool _logout;

    [RelayCommand]
    private void Closed()
    {
        if (Close)
        {
            this.Result = CloseEnum.Close;
        }
        else if (Mini)
        {
            this.Result = CloseEnum.Notify;
        }
        else if (Logout)
        {
            this.Result = CloseEnum.Logout;
        }

        Dialog.Close(HcDialogMessageToken.DialogMainToken);
        this.action?.Invoke(this.Result);
    }

    [RelayCommand]
    private void Cancel()
    {
        this.Result = CloseEnum.None;
        Dialog.Close(HcDialogMessageToken.DialogMainToken);
    }
}