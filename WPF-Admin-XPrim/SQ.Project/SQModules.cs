using SQ.Project.ViewModels;
using SQ.Project.Views;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.I18n;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace SQ.Project
{
    [Module("SQ.Project")]
    public class SQModules : IModule
    {
        public static Func<string, string> t;

        static SQModules()
        {
            (t, _) = CSharpI18n.UseI18n();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
           
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var regionManager = containerRegistry.Resolve<IRegionManager>();
            regionManager.RegisterForNavigation<SQVisualHome, SQVisualHomeViewModel>(RegionName.HomeRegion,
                "SQVisual");
           
        }
    }
}
