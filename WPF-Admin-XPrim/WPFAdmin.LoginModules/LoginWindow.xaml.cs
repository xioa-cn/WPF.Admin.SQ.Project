using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Themes;
using XPrism.Core.DI;

namespace WPFAdmin.LoginModules;

public partial class LoginWindow : Window {
    public LoginWindow() {
        this.DataContext = new LoginViewModel();
        InitializeComponent();
        // this.Closed += ClosedMethod;
        InitializedVm();
    }

    private void InitializedVm()
    {
        if(this.DataContext is LoginViewModel vm)
        {
            var remUser = LoginAuthHelper.GetRemUserMes();

            if (remUser != null)
            {
                vm.InputText = remUser.Account;
                //vm.Password = remUser.Password;
                vm.RememberPassword = remUser.IsRem;
                this.pwd.Password = remUser.Password;
            }
        }
        
    }

    public void SuccessLogin() {
        var mainWindow =
            XPrismIoc.FetchXPrismWindow("MainWindow");
        WeakReferenceMessenger.Default.Send(new UseIcon());
        WeakReferenceMessenger.Default.Send<RefreshTree>(new RefreshTree
        {
            Refresh = true
        });
        this.SwitchWindow(mainWindow);
    }

    private void ClosedMethod(object? sender, EventArgs e) {
        this.CloseWindowWithFade();
        Environment.Exit(0);
    }

    private void CloseWindow_Click(object sender, RoutedEventArgs e) {
        this.Close();
        this.CloseWindowWithFade();
        XPrism.Core.Co.CloseApplication.ShutdownApplication();
        Application.Current.Shutdown();
        Environment.Exit(0);
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
        // 使用默认浏览器打开链接
        Process.Start(new ProcessStartInfo {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });

        // 标记事件已处理
        e.Handled = true;
    }
}