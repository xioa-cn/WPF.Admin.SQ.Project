using PressMachineCMS.Config;
using PressMachineCMS.ViewModels;
using PressMachineCMS.Views;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace PressMachineCMS {
    [Module("PressMachineCMS")]
    public class CMSModule : IModule {
        public void RegisterTypes(IContainerRegistry containerRegistry) {
            var regionManager = containerRegistry.Resolve<IRegionManager>();
            regionManager.RegisterForNavigation<VisualSettings, VisualSettingsViewModel>(RegionName.HomeRegion,
                "VisualSettings");
            regionManager.RegisterForNavigation<RouterSettings, RouterSettingsViewModel>(RegionName.HomeRegion,
                "RouterSettings");
            regionManager.RegisterForNavigation<PlcSettings, PlcSettingsViewModel>(RegionName.HomeRegion,
                "PlcSettings");
        }

        public async void OnInitialized(IContainerProvider containerProvider) {
            await using CMSSettingsDbContext cmsSettingsDbContext =
                new CMSSettingsDbContext(CMSConfig.Instance.ConfigDir);
            if (!System.IO.File.Exists(cmsSettingsDbContext.Dir))
            {
                cmsSettingsDbContext.Database.EnsureCreated();
            }
        }
    }
}