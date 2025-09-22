using System.IO;
using PressMachineMainModeules.Config;
using WPF.Admin.Models.Background;
using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Utils;
using XPrism.Core.DI;
using XPrism.Core.Navigations;
using Application = System.Windows.Application;

namespace PressMachineMainModeules;

[XPrism.Core.Modules.Find.Module(nameof(PressMachineModules))]
public class PressMachineModules : XPrism.Core.Modules.IModule
{
    private readonly static bool StartupAuto = true;

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        var regionManager = containerRegistry.Resolve<IRegionManager>();

        if (PressMachineModules.StartupAuto)
        {
            regionManager
                .RegisterForNavigation<PressMachineMainModeules.Views.AutoHomeView,
                    PressMachineMainModeules.ViewModels.AutoHomeViewModel>(
                    WPF.Admin.Models.Models.RegionName.HomeRegion,
                    "AutoHome");
            regionManager
                .RegisterForNavigation<PressMachineMainModeules.Views.AutoParameterPage,
                    PressMachineMainModeules.ViewModels.AutoParameterViewModel>(
                    WPF.Admin.Models.Models.RegionName.HomeRegion,
                    "Params");
            regionManager
                .RegisterForNavigation<PressMachineMainModeules.Views.AutoMesTemplateView,
                    PressMachineMainModeules.ViewModels.AutoMesTemplateViewModel>(
                    WPF.Admin.Models.Models.RegionName.HomeRegion,
                    "AutoMes");
            regionManager
                .RegisterForNavigation<PressMachineMainModeules.Views.AutoCheckCodeView,
                    PressMachineMainModeules.ViewModels.AutoWorkwearCheckCodeViewModel>(
                    WPF.Admin.Models.Models.RegionName.HomeRegion,
                    "AutoCheckCode");
            regionManager.RegisterForNavigation<
                PressMachineMainModeules.Views.BarcodeCharacteristics,
                PressMachineMainModeules.ViewModels.BarcodeCharacteristicsViewModel>(
                WPF.Admin.Models.Models.RegionName.HomeRegion,
                "BarcodeCharacteristics");
        }
        else
        {
            regionManager
                .RegisterForNavigation<PressMachineMainModeules.Views.Home,
                    PressMachineMainModeules.ViewModels.HomeViewModel>(WPF.Admin.Models.Models.RegionName.HomeRegion,
                    "Home");
            regionManager
                .RegisterForNavigation<PressMachineMainModeules.Views.PreParams,
                    PressMachineMainModeules.ViewModels.PressMachineParamsViewModel>(
                    WPF.Admin.Models.Models.RegionName.HomeRegion,
                    "Params");
        }


        regionManager
            .RegisterForNavigation<PressMachineMainModeules.Views.PreHisPlot,
                PressMachineMainModeules.ViewModels.PreHisPlotViewModel>(WPF.Admin.Models.Models.RegionName.HomeRegion,
                "HisPlot");
        regionManager
            .RegisterForNavigation<PressMachineMainModeules.Views.PreHisTable,
                PressMachineMainModeules.ViewModels.PreHisTableViewModel>(WPF.Admin.Models.Models.RegionName.HomeRegion,
                "Table");
        // PLC信号交互
        regionManager
            .RegisterForNavigation<PressMachineMainModeules.Views.SignalInteractionView,
                PressMachineMainModeules.ViewModels.SignalInteractionViewModel>(WPF.Admin.Models.Models.RegionName.HomeRegion,
                "Signal");
        regionManager
            .RegisterForNavigation<PressMachineMainModeules.Views.PreManual,
                PressMachineMainModeules.ViewModels.PreManualViewModel>(WPF.Admin.Models.Models.RegionName.HomeRegion,
                "Manual");
        regionManager
            .RegisterForNavigation<PressMachineMainModeules.Views.PreSystemManager,
                PressMachineMainModeules.ViewModels.PreSystemManagerViewModel>(
                WPF.Admin.Models.Models.RegionName.HomeRegion,
                "PreManager");
        regionManager
            .RegisterForNavigation<PressMachineMainModeules.Views.PreManual2,
                PressMachineMainModeules.ViewModels.PreManual2ViewModel>(WPF.Admin.Models.Models.RegionName.HomeRegion,
                "Manual2");
        regionManager
            .RegisterForNavigation<PressMachineMainModeules.Views.IoManagerPage,
                PressMachineMainModeules.ViewModels.IoManagerViewModel>(
                WPF.Admin.Models.Models.RegionName.HomeRegion, "IoManager");
    }

    public async void OnInitialized(IContainerProvider containerProvider)
    {
        PressMachineMainModeules.Utils.PressMachineDataContext db =
            new PressMachineMainModeules.Utils.PressMachineDataContext();
        if (!System.IO.Directory.Exists(db.DbPath))
        {
            System.IO.Directory.CreateDirectory(db.DbPath);
        }

        db.Database.EnsureCreated();
        
        await using var sysDb = new SignalDbContext();
        await sysDb.DbFileExistOrCreate();
        
        // 初始化报警
        PressMachineMainModeules.ViewModels.AlarmViewModel alarm =
            new PressMachineMainModeules.ViewModels.AlarmViewModel();
        alarm.Initialized();
        // 初始化自动plc
        if (PressMachineModules.StartupAuto)
        {
            if (!PressMachineModuleHelper.PressMachineExcelConfigExist())
            {
                var sri = ApplicationUtils
                    .FindApplicationResourceStreamInfo("WPFAdmin", "IOConfigs/PressMachineConfig.xlsx");

                await using var sourceStream = sri.Stream;
                await using var destinationStream = System.IO.File.Create(ConfigPlcs.ConfigPath);
                await sourceStream.CopyToAsync(destinationStream);
            }

            PressMachineMainModeules.Models.AutoMesConfigManager.Instance.Initialized();
            PressMachineMainModeules.Models.PlotConfigManager.Instance.Initialized();
            PressMachineMainModeules.Models.HomeManager.Instance.Initiailzed();
            PressMachineMainModeules.Utils.AutoParameterModelManager.Instance.Initialized();
            PressMachineMainModeules.Config.ConfigPlcs.Instance.Initialized();
        }

        QueuedHostedService.TaskQueue.QueueBackgroundWorkItem(async (token) =>
        {
            Console.WriteLine("PressMachineModules initialized");
        });
    }
}