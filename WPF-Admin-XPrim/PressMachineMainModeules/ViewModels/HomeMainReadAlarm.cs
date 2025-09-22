
using HandyControl.Controls;
using HslCommunication.Core.Device;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.ViewModels
{
    public partial class HomeViewModel : BindableBase
    {
        public void ReadAlarm()
        {
            if (Common.OpenIO)
            {
                AlarmManager.Instance.Initialized(ConfigPlcs.ConfigPath,"Alarm");
            }
            else
            {
                return;
            }

            while (true)
            {
                try
                {

                    var alarmList = new List<Tuple<string, bool>>();

                    foreach (var item in AlarmManager.Instance.AlarmPositions)
                    {
                        var state = ConfigPlcs.Instance[item.PlcName].ReadBool(item.getFullPosition);
                        if (state.IsSuccess)
                        {
                            alarmList.Add(new Tuple<string, bool>(item.getFullPosition, state.Content));
                        }
                        else
                        {
                            throw new Exception($"IO读取失败，点位：{item.getFullPosition}，错误信息：{state.Message}");
                        }
                    }

                    var sendData = alarmList.Where(x => x.Item2 == true).Select(x => x.Item1).ToList();
                    if (sendData.Count > 0)
                    {
                        AlarmToUIModels.CreateNormalAlarm(sendData).Send();
                    }

                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError("IO读取异常", ex);
                    Growl.ErrorGlobal("IO读取异常:" + ex.Message);

                }
                Thread.Sleep(500);
            }
        }
    }
}
