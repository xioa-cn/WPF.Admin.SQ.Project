using CommunityToolkit.Mvvm.ComponentModel;


namespace WPF.Admin.Models.Models
{
    public partial class UILoginUser:BindableBase
    {
        [ObservableProperty] private int _Id;
        [ObservableProperty] private string? _UserName;
        [ObservableProperty] private string? _Password;
        [ObservableProperty] private DateTime _CreateTime;
        [ObservableProperty] private string? _Header;
        [ObservableProperty] private LoginAuth _LoginAuth;
    }
}
