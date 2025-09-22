using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Models.Models;

public partial class TreeItemModel : BindableBase
{
    /// <summary>
    /// 持久化使用组件
    /// </summary>
    [JsonPropertyName("isPersistence")]
    public bool IsPersistence { get; set; } = true;
    [JsonPropertyName("key")]
    public string? Key { get; set; }
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("LoginAuth")]
    [ObservableProperty] public LoginAuth _LoginAuth = LoginAuth.None;
    [JsonPropertyName("icon")] public string? Icon { get; set; }
    [JsonPropertyName("page")] public string? Page { get; set; }

    [ObservableProperty] private bool _isChecked;

    [ObservableProperty] private bool _isExpanded;
    [JsonPropertyName("children")] public ObservableCollection<TreeItemModel> Children { get; set; } = new();

    public bool HasChildren => Children.Count > 0;
    [ObservableProperty]
    private bool _isEnabled = false;
    [ObservableProperty]
    private bool _visibility = true;

    public PageCanInterchange PageCanInterchange { get; set; } = PageCanInterchange.Can;

    public TreeItemModel()
    {
        WeakReferenceMessenger.Default.Register<LoginAuthManager>(this, (r, m) =>
                                   {
                                       if ((int)m.Login <= (int)this.LoginAuth)
                                       {
                                           if (AuthHelper.ViewAuthSwitch == ViewAuthSwitch.IsEnabled)
                                               IsEnabled = true;
                                           if (AuthHelper.ViewAuthSwitch == ViewAuthSwitch.Visibility)
                                               Visibility = true;
                                       }
                                       else
                                       {
                                           if (AuthHelper.ViewAuthSwitch == ViewAuthSwitch.IsEnabled)
                                               IsEnabled = false;
                                           if (AuthHelper.ViewAuthSwitch == ViewAuthSwitch.Visibility)
                                               Visibility = false;
                                       }
                                   });
    }

    [ObservableProperty] private string? _message;

    public bool HasMessage => !string.IsNullOrEmpty(_message);

    public PageStatus PageStatus { get; set; } = PageStatus.Page;

    public TreeItemModel(string con)
    {
        this.Content = con;
    }
}