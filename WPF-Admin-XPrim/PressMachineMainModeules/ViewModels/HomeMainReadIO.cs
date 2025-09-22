using HandyControl.Controls;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.ViewModels
{
    public partial class HomeViewModel : BindableBase
    {
        public void ReadIo()
        {
            if (Common.OpenIO)
            {
                // 初始化手动界面IO
                IOManager.ManualIOInstance.Initialized(ConfigPlcs.ConfigPath, "Alarm");
                // 初始化IO
                IOManager.IOInstance.Initialized(ConfigPlcs.ConfigPath, "Alarm");
            }
            else
            {
                return;
            }

            while (true)
            {
                try
                {

                    foreach (var item in IOManager.IOInstance.Instance)
                    {
                        var state = ConfigPlcs.Instance[item.PlcName].ReadBool(item.Point);
                        if (state.IsSuccess)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                item.State = state.Content;
                            });
                        }
                        else
                        {
                            throw new Exception($"IO读取失败，点位：{item.Point}，错误信息：{state.Message}");
                        }
                        Thread.Sleep(1);
                    }

                    foreach (var item in IOManager.ManualIOInstance.Instance)
                    {
                        var state = ConfigPlcs.Instance[item.PlcName].ReadBool(item.Point);
                        if (state.IsSuccess)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                item.State = state.Content;
                            });
                        }
                        else
                        {
                            throw new Exception($"IO读取失败，点位：{item.Point}，错误信息：{state.Message}");
                        }
                        Thread.Sleep(1);
                    }

                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError("IO读取异常", ex);
                    Growl.ErrorGlobal("IO读取异常:" + ex.Message);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
