using System.ComponentModel;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Themes;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;
using WPF.Admin.Themes.Themes;
using WPFAdmin.Config;
using WPFAdmin.LoginModules;
using WPFAdmin.ViewModels;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Navigations;

namespace WPFAdmin.Views;

[XPrismViewModel(nameof(MainWindow))]
public partial class MainWindow
{
    private INavigationService _navigationService;
    private readonly GlobalHotKey? _globalHotKey;

    public MainWindow(INavigationService navigationService)
    {
        this.SetWindowIcon();
        _navigationService = navigationService;
        if (Configs.Default != null && Configs.Default.GlobalHotKey)
        {
            _globalHotKey = new GlobalHotKey(this);
        }

        Dialog.Register(HcDialogMessageToken.DialogMainToken, this);
        InitializeComponent();
        navigationService.NavigateAsync("MainRegion/Main");
        this.Loaded += (_, _) =>
        {
            try
            {
                if (Configs.Default is not null && Configs.Default.ThemeCursor)
                {
                    this.UseNormalCursor();
                }

                if (_globalHotKey is null) return;
                _globalHotKey.RegisterHotKey(
                    GlobalHotKey.ModControl | GlobalHotKey.ModAlt,
                    'T',
                    ThemeManager.ChangeDarkTheme);
                _globalHotKey.RegisterHotKey(
                    GlobalHotKey.ModAlt,
                    'T',
                    ThemeManager.ChangeDarkTheme);
            }
            catch (Exception exception)
            {
                XLogGlobal.Logger?.LogError(exception.Message, exception);
            }
        };
        //this.WindowState = WindowState.Maximized;
        //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        if (Configs.Default?.WindowSize != "Max")
        {
            return;
        }

        this.Top = 0;
        this.Left = 0;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        this.CloseApplication();
    }

    private void CloseApplication()
    {
        App.DisposeAppResources();
        XPrism.Core.Co.CloseApplication.ShutdownApplication();
    }

    private void MiniSize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaxSize_Click(object sender, RoutedEventArgs e)
    {
        //if (Config.Configs.Default?.WindowSize == "Max") return;
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
        }

        else if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
        }
    }

    private NotifyIconView? _notifyIconView;

    private async void Close_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _notifyIconView ??= new NotifyIconView(async void (closeEnum) =>
            {
                try
                {
                    switch (closeEnum)
                    {
                        case CloseEnum.Close:
                            App.DisposeAppResources();
                            this.CloseWindowWithFade();
                            Environment.Exit(0);
                            break;
                        case CloseEnum.Notify:
                            await Task.Delay(100);
                            this.Visibility = Visibility.Hidden;
                            break;
                        case CloseEnum.Logout:
                            GoBackLoginView();
                            break;
                        case CloseEnum.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception exception)
                {
                    XLogGlobal.Logger?.LogError(exception.Message, exception);
                }
            });

            var dialog = Dialog.Show(_notifyIconView, HcDialogMessageToken.DialogMainToken);
            var temp = dialog.Initialize<NotifyIconViewModel>(vm => { });
            await temp.GetResultAsync<CloseEnum>()
                .ContinueWith(re => { });
        }
        catch (Exception ex)
        {
            XLogGlobal.Logger?.LogError(ex.Message, ex);
        }
    }

    private void GoBackLoginView()
    {
        App.DisposeNotifyIconResources(); // 清理图标防止从图标打开主界面
        LoginAuthHelper.LoginUser = null; // 注销权限

        var login = XPrismIoc.Fetch<LoginWindow>();
        if (login is not null)
        {
            this.Visibility = Visibility.Hidden;
            login.Show();
        }
        else
        {
            Growl.ErrorGlobal("Not Found LoginWindow");
        }
    }
}