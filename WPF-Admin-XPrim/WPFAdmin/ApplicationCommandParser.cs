using System.Windows;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Themes.Themes;
using WPFAdmin.Config;
using WPFAdmin.Utils;
using WPFAdmin.ViewModels;
using WPFAdmin.Views;

namespace WPFAdmin;

public partial class App {
    private CommandParser _commandLine;

    private void EnableDebugMode() {
        // 调试模式逻辑
    }

    private ApplicationStartupMode StartupCommandLine(string[]? args, Window window) {
        if (args is null || args.Length < 1)
        {
            return ApplicationStartupMode.Normal;
        }

        // --debug  -width 1024 -height 768 -maximize true
        _commandLine = new CommandParser(args);
        if (!_commandLine.HasParameter("debug")) return ApplicationStartupMode.Normal;

        var config = _commandLine.GetValue("config", "");

        var resetAdmin = _commandLine.GetValue("account", "");

        var cms = _commandLine.GetValue("open", "");

        if (cms == "cms")
        {
            return StartupCMS("cmsRouter");
        }

        if (cms == "cmsAppSettings")
        {
            return StartupCMS("appSettingsRouter");
        }

        if (cms == "cmsRouterAppSettings")
        {
            return StartupCMS("cmsRouterRouter");
        }


        // --debug -account resetAdmin // 重置Admin密码
        if (resetAdmin == "resetAdmin")
        {
            MessageBox.Show("已启动重置账户程序！");
            XLogGlobal.Logger?.LogWarning("DEBUG MODE ========> resetAdmin");
            using SysDbContent db = new SysDbContent();
            var findInfo = db.LoginUsers.FirstOrDefault(x => x.UserName == "Admin");

            if (findInfo is not null)
            {
                MessageBox.Show("找到Admin账户");
                findInfo.Password = "123456";
                db.SaveChanges();
            }
            else
            {
                MessageBox.Show("新建Admin账户");
                db.LoginUsers.Add(new LoginUser {
                    UserName = "Admin",
                    Password = "123456",
                    LoginAuth = LoginAuth.Admin
                });
                db.SaveChanges();
            }

            MessageBox.Show("重置密码成功");
        }

        if (config == "window")
        {
            ConfigWindow configWindow = new ConfigWindow();
            window.SwitchWindow(configWindow);
            //configWindow.ShowDialog();
        }

        EnableDebugMode();
        return ApplicationStartupMode.Debug;
    }

    private ApplicationStartupMode StartupCMS(string cmsString) {
        if (Configs.Default is null)
            throw new ArgumentNullException(nameof(Configs.Default));
        Configs.Default.RouteName = cmsString;
        XLogGlobal.Logger?.LogWarning("DEBUG MODE ========> cms");
        AppSettings.ApplicationCms = ApplicationCms.BatCms;
        return ApplicationStartupMode.CMS;
    }
}