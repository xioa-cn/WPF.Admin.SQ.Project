using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using HandyControl.Controls;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Themes.Converter;
using XPrism.Core.Navigations;

namespace WPFAdmin.NavigationModules.ViewModel;

public partial class MainViewModel : BindableBase
{
    public ObservableCollection<TreeItemModel>? TreeItems { get; set; }
    public INavigationService NavigationService { get; set; }


    public MainViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        RefreshTree();
        WeakReferenceMessenger.Default.Register<RefreshTree>(this, RefreshTreeMethods);

        var createTime = ApplicationAuthModule.DllCreateTime;

        ApplicationCreateTime =
            $"{createTime.ToString("yyyy")} © {nameof(WPFAdmin)} BY {AppAuthor.Author}. {createTime.TimeYearMonthDayHourString()}";
    }

    private void RefreshTreeMethods(object recipient, RefreshTree message)
    {
        if (message.Refresh)
            RefreshTree();
    }

    private bool _index = true;

    private async void RefreshTree()
    {
        this.TreeItems = new ObservableCollection<TreeItemModel>();
        var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            $"{AppSettings.Default?.RouteName}.json");

        if (!System.IO.File.Exists(file))
        {
            Growl.ErrorGlobal("检测到路由文件丢失，尝试初始化路由");
            XLogGlobal.Logger?.LogError("检测到路由文件丢失，尝试初始化路由");
            var sri = Application.GetResourceStream(
                new Uri($"pack://application:,,,/WPFAdmin;component/{AppSettings.Default?.RouteName}.json"));
            if (sri == null)
            {
                throw new Exception("Route file not found");
            }

            using StreamReader reader = new StreamReader(sri.Stream);

            string result = await reader.ReadToEndAsync();
            await System.IO.File.WriteAllTextAsync(file, result);
        }

        var read = await System.IO.File.ReadAllTextAsync(file);
        var data = System.Text.Json.JsonSerializer.Deserialize<Router>(read);
        if (data?.Routers == null) return;
        foreach (var item in data.Routers)
        {
            this.TreeItems.Add(item);
        }

        WeakReferenceMessenger.Default.Send(
            LoginAuthManager.Create(LoginAuthHelper.LoginUser?.LoginAuth ?? LoginAuth.None));
        
        if (!_index)
        {
            return;
        }

        await NavigationService.NavigateAsync($"{RegionName.HomeRegion}/BasePage");
        //Thread.Sleep(50);
        WeakReferenceMessenger.Default.Send(OpenNavIndexHelper.NavIndexConfig);
        _index = false;
    }


    private static UILoginUser? _loginUser = null;

    public static UILoginUser? LoginUser
    {
        get => _loginUser;
        set
        {
            AuthChangeView();
            _loginUser = value;
            LoginAuthHelper.LoginUser = value;
        }
    }

    private static void AuthChangeView()
    {
    }


    [ObservableProperty] private string _ApplicationCreateTime;
}