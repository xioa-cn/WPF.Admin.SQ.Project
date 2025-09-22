using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using System.Collections.ObjectModel;
using WPF.Admin.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.ViewModels
{
    public partial class IoManagerViewModel : BindableBase
    {
        [ObservableProperty] private ObservableCollection<IOPointPositionModel>? _IOPointPositionModels;
        public bool ViewIsLoaded { get; set; } = false;
        public IoManagerViewModel()
        {
            if (Common.OpenIO)
            {
                Task.Run(() =>
                {
                    // 初始化手动界面IO
                    IOManager.IOInstance.Initialized(ConfigPlcs.ConfigPath, "IOState");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IOPointPositionModels = IOManager.IOInstance.Instance;
                    });

                });
                Task.Factory.StartNew(IOCommn, TaskCreationOptions.LongRunning);
            }

        }

        private void IOCommn()
        {
            while (true)
            {
                try
                {
                    if (IOPointPositionModels is not null && ViewIsLoaded)
                        foreach (var item in IOPointPositionModels)
                        {
                            var value = ConfigPlcs.Instance[item.PlcName]?.ReadBool(item.Point);

                            if (value is not null && value.IsSuccess)
                            {
                                item.State = value.Content;
                            }
                            else
                            {
                                var message = $"ReadFloat Error: {value?.Message} \nPosition: {item.Point} \nPlcName: {item.PlcName}";
                                throw new Exception(message);
                            }
                        }
                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError(ex.Message, ex);
                    Growl.WarningGlobal($"ManualValueRefresh Error: {ex.Message}");
                    Thread.Sleep(5000);
                }
                Thread.Sleep(500);
            }
        }
    }
}
