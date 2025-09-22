using SystemModules.ViewModels;
using SystemModules.Views;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.I18n;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace SystemModules
{
    [Module(nameof(SystemModules))]
    public class SystemModules : IModule
    {
        public static Func<string, string> t;

        static SystemModules()
        {
            (t, _) = CSharpI18n.UseI18n();
        }

        public async void OnInitialized(IContainerProvider containerProvider)
        {
            await using var sysDb = new SysDbContent();
            await sysDb.DbFileExistOrCreate();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var regionManager = containerRegistry.Resolve<IRegionManager>();
            regionManager.RegisterForNavigation<SystemUserView, SystemUserViewModel>(RegionName.HomeRegion,
                "SystemUser");
            regionManager.RegisterForNavigation<SystemAlarmLogView, SystemAlarmLogViewModel>(RegionName.HomeRegion,
                "SystemAlarmLog");
        }
    }
}