using CMS.AppRouters.Models;
using CMS.AppRouters.ViewModels;
using CMS.AppRouters.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace CMS.AppRouters {
    [Module(nameof(CMSAppRoutersModule))]
    public class CMSAppRoutersModule: IModule {
        public void RegisterTypes(IContainerRegistry containerRegistry) {
            var regionManager = containerRegistry.Resolve<IRegionManager>();
            regionManager.RegisterForNavigation<RoutersSettingsView,RoutersSettingsViewModel>(RegionName.HomeRegion,
                "CMSRoutersSettings");
        }

        public void OnInitialized(IContainerProvider containerProvider) {
            AppNormalRouter.Initialized();
        }
    }
}