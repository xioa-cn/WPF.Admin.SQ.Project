using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using WPF.Admin.Models;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Impl;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.I18n;

namespace WPFAdmin.LoginModules;

public partial class LoginViewModel : BindableBase
{
    [ObservableProperty] private string? _InputText;

    public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>()
    {
    };

    [ObservableProperty] private string? _userName;
    [ObservableProperty] private string? _password;
    [ObservableProperty] private bool _rememberPassword;

    public LoginViewModel()
    {
        try
        {
            (t, _) = CSharpI18n.UseI18n();
            using var db = new SysDbContent();

            if (db is ISqliteNormalable sqlite)
            {
                sqlite.DbFileExistOrCreate().Wait();
            }

            var find = db.LoginUsers.AsNoTracking().ToList().OrderBy(e => e.LoginAuth).Select(e => e.UserName);

            if (!find.Any())
            {
                return;
            }

            Items.Clear();
            foreach (var item in find)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
        }
    }

    [RelayCommand]
    private void Delete(string value)
    {
        Items.Remove(value);
    }

    [RelayCommand]
    private async Task Login(System.Windows.Window window)
    {
        if (string.IsNullOrWhiteSpace(InputText))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            return;
        }

        window.IsEnabled = false;

        try
        {
            using var db = new SysDbContent();

            var find = await db.LoginUsers.FirstOrDefaultAsync(e => e.UserName == InputText);

            if (find == null)
            {
                throw new Exception(t!("Login.NotFountUser"));
            }

            if (find.Password == this.Password)
            {
                if (LoginAuthHelper.LoginUser is not null)
                {
                    LoginAuthHelper.LoginUser.UserName = find.UserName;
                    LoginAuthHelper.LoginUser.Password = find.Password;
                    LoginAuthHelper.LoginUser.LoginAuth = find.LoginAuth;
                }
                else
                {
                    LoginAuthHelper.LoginUser = new UILoginUser()
                    {
                        UserName = find.UserName,
                        Password = find.Password,
                        LoginAuth = find.LoginAuth,
                    };
                }


                (window as LoginWindow).SuccessLogin();

                Growl.SuccessGlobal($"Login Success!! {InputText}");

                Task.Run(() =>
                {
                    //记住密码
                    LoginAuthHelper.RememberUserToConfig(this.RememberPassword);
                });
                return;
            }

            throw new Exception(t!("Login.PasswordError"));
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        window.IsEnabled = true;
    }
}