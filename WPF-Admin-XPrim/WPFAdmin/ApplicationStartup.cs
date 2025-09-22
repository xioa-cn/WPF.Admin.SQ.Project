using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using External.GrpcServices.Utils;
using HandyControl.Controls;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services.Garnets;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Themes;
using WPFAdmin.Config;
using WPFAdmin.LoginModules;
using XPrism.Core.DI;

namespace WPFAdmin;

public partial class App {
    private void StartupWindow(Views.SplashScreen splashScreen,
        ApplicationStartupMode applicationStartupMode = ApplicationStartupMode.Normal) {
        var s = Enum.TryParse<IndexStatus>(Configs.Default?.IndexStatus, out var indexStatus);
        StartupGrpc();
        if (!s)
        {
            MessageBox.Show("初始化窗口异常！！！", "Error");
            Environment.Exit(0);
            return;
        }

        if (Configs.Default is not null && Configs.Default.IsRememberThePasswordToLogInAutomatically)
            if (indexStatus == IndexStatus.Main)
            {
                if (System.IO.File.Exists(LoginAuthHelper.path))
                {
                    try
                    {
                        var resultUser = SerializeHelper.Deserialize<Remuser>(LoginAuthHelper.path);
                        if (resultUser is null || !resultUser.IsRem)
                            indexStatus = IndexStatus.Login;
                        if (resultUser is not null)
                        {
                            var userDb = new SysDbContent();
                            var userInfo = userDb.LoginUsers.FirstOrDefault(e => e.UserName == resultUser.Account);
                            if (userInfo is not null)
                            {
                                LoginAuthHelper.LoginUser = new UILoginUser() {
                                    LoginAuth = userInfo.LoginAuth,
                                    UserName = userInfo.UserName,
                                    Password = userInfo.Password,
                                    CreateTime = userInfo.CreateTime
                                };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        XLogGlobal.Logger?.LogError(e.Message, e);
                        indexStatus = IndexStatus.Login;
                    }
                }
                else
                {
                    indexStatus = IndexStatus.Login;
                }
            }

        // 先加载MainViewModel 防止权限更新监听没有更新
        var mainWindow =
            XPrismIoc.FetchXPrismWindow(nameof(MainWindow));

        switch (indexStatus)
        {
            case IndexStatus.Login: {
                var login = XPrismIoc.Fetch<LoginWindow>();
                splashScreen.SwitchWindow(login);
                WeakReferenceMessenger.Default.Register<UseIcon>(this, OpenIcon);
                break;
            }
            case IndexStatus.Main: {
                // 直接进入main窗口时 查看上次登录的权限
                AuthLoaded();
                splashScreen.SwitchWindow(mainWindow);
                NotifyIconInitialize();
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        OpenGarnetService();
    }

    private static Process? Process;

    private void OpenGarnetService() {
        if (Configs.Default is not null && Configs.Default.OpenGarnet)
        {
            Task.Run(() => { GarnetService.StartupGarnet(Configs.Default.GarnetPort); });
        }
    }

    private void StartupGrpc() {
        if (Configs.Default is not null && Configs.Default.OpenExternalGrpc)
        {
            Task.Run(() =>
            {
                ApplicationGrpc.AppGrpcServices();
                // ProcessStartInfo startInfo = new ProcessStartInfo();
                // startInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                //     , "WPFAdmin.rpc.bat");
                // startInfo.WindowStyle = ProcessWindowStyle.Normal;
                // Process = new Process();
                // Process.StartInfo = startInfo;
                // try
                // {
                //     // 启动进程
                //     Process.Start();
                //     Process.WaitForExit();
                //     int exitCode = Process.ExitCode;
                // }
                // catch (Exception ex)
                // {
                //     
                // }
            });
        }
    }

    private void AuthLoaded() {
        var remUser = LoginAuthHelper.GetRemUserMes();


        if (remUser != null)
        {
            using var db = new SysDbContent();
            var find = db.LoginUsers.FirstOrDefault(e => e.UserName == remUser.Account);

            if (find is not null)
                LoginAuthHelper.LoginUser = new UILoginUser() {
                    UserName = find.UserName,
                    Password = find.Password,
                    LoginAuth = find.LoginAuth,
                };
        }

        else
        {
            LoginAuthHelper.LoginUser = new UILoginUser() {
                UserName = "游客",
                Password = "",
                LoginAuth = LoginAuth.Employee,
            };
        }
    }
}