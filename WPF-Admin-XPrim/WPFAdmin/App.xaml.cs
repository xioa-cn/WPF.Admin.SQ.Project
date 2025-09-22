using System.Reflection;
using System.Windows;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Service.Services.Garnets;
using WPF.Admin.Service.Utils;
using WPF.Admin.Themes;
using WPF.Admin.Themes.CodeAuth;
using WPF.Admin.Themes.Monitor;
using WPF.Admin.Themes.Themes;
using WPFAdmin.Config;
using WPFAdmin.LoginModules;
using WPFAdmin.Utils;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Core.Modules.Find;

namespace WPFAdmin;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        ConfigureLangManager();
        ApplicationAuthModule.DllCreateTime = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location);
        AppBaseConfigHelper.InitializedAppBaseConfig();
        //ListenApplicationVersions.NormalVersion = ApplicationVersions.NoAuthorization;
        DispatcherHelper.Initialize();
        ThemeManager.SetTheme(Configs.Default?.Theme);
        Task.Run(BatFileHelper.CreateBatFile);
        ApplicationCodeAuth.Startup();
        GlobalExceptionHandle();

        // 初始化服务
        UsingHostServices();
        if (Configs.Default is not null && Configs.Default.OpenTrackingManager)
        {
            // 埋点监控
            TrackingManager.Instance.Initialize();
        }

        Detect();
        base.OnStartup(e);
        GarnetService.StartupCreateExeGarnet();
        Views.SplashScreen splashScreen = new Views.SplashScreen();
        splashScreen.ShowWindowWithFade();
        await Task.Run(() =>
        {
            Thread.Sleep(5000);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ContainerLocator.Container.RegisterTransient<LoginWindow>();
                ContainerLocator.Container.RegisterEventAggregator<EventAggregator>();
                ContainerLocator.Container.AutoRegisterByAttribute(Assembly.Load("WPFAdmin"));
                ContainerLocator.Container
                    .RegisterSingleton<IModuleFinder>(new DirectoryModuleFinder())
                    .RegisterMeModuleManager(manager =>
                    {
                        manager.LoadModulesConfig(AppDomain.CurrentDomain.BaseDirectory);
                    });
                ContainerLocator.Container.AutoRegisterByAttribute<XPrismViewModelAttribute>(
                    Assembly.Load("WPFAdmin"));
                ContainerLocator.Container.Build();

                var applicationStartupMode = StartupCommandLine(e.Args, splashScreen);

                if (applicationStartupMode == ApplicationStartupMode.Debug)
                {
                    Environment.Exit(0);
                }
                else
                {
                    StartupWindow(splashScreen, applicationStartupMode);
                }
            });
        });

        XLogGlobal.Logger?.LogFatal("打开了软件");
    }
}