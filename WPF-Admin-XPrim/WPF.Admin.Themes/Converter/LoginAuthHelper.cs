using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Themes.Converter;

public class Remuser {
    public string Account { get; set; }
    public string Password { get; set; }
    public bool IsRem { get; set; }
}

public class LoginAuthHelper {
    public static ViewAuthSwitch ViewAuthSwitch { get; set; }

    private static UILoginUser? _loginUser;

    public static UILoginUser? LoginUser {
        get => _loginUser;
        set
        {
            if (_loginUser is not null && value is not null)
            {
                _loginUser.LoginAuth = value.LoginAuth;
                _loginUser.UserName = value.UserName;
                _loginUser.Password = value.Password;
            }
            else if (value is not null)
            {
                _loginUser = value;
            }
            else
            {
                _loginUser.LoginAuth = LoginAuth.None;
            }

            if (value is not null)
                // 通知修改权限
                WeakReferenceMessenger.Default.Send(
                    LoginAuthManager.Create(value?.LoginAuth ?? LoginAuth.None));
        }
    }

    public static void SetViewAuthSwitch(string? viewAuthSwitch) {
        var res = Enum.TryParse<ViewAuthSwitch>(viewAuthSwitch, out var auth);
        if (res)
        {
            ViewAuthSwitch = auth;
            AuthHelper.ViewAuthSwitch = auth;
        }
    }

    public static string path =>
        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "user.json");

    public static void RememberUserToConfig(bool isRem = true) {
        Remuser remuser = new Remuser() {
            IsRem = isRem,
            Account = LoginUser.UserName,
            Password = LoginUser.Password,
        };
        SerializeHelper.Serialize(path, remuser);
    }


    public static Remuser? GetRemUserMes() {
        if (!System.IO.File.Exists(path))
        {
            return null;
        }


        var resultUser = SerializeHelper.Deserialize<Remuser>(path);

        if (resultUser.IsRem)
        {
            return resultUser;
        }

        return null;
    }
}