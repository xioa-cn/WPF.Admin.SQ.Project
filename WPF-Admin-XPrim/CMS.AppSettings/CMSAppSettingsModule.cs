using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace CMS.AppSettings
{
    [Module(nameof(CMSAppSettingsModule))]
    public class CMSAppSettingsModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var regionManager = containerRegistry.Resolve<IRegionManager>();
            regionManager.RegisterForNavigation<CMS.AppSettings.Views.AppSettings,
                CMS.AppSettings.ViewModels.AppSettingsViewModel>(RegionName.HomeRegion,
                "AppSettings");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}