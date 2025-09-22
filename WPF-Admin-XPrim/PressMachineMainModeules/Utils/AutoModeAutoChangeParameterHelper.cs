using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.ViewModels;
using WPF.Admin.Models.Utils;
using WPF.Admin.Themes.Converter;

namespace PressMachineMainModeules.Utils
{
    public class AutoModeAutoChangeParameterHelper
    {
        private static string oldParameterName = "";

        public static async Task ChangeParameterGlobal(string parameterName)
        {
            if (oldParameterName == parameterName) return;
            Growl.Success("正在自动切换配方,请稍后...");
            string filename = AutoParameterViewModel.dir + $"\\{parameterName}.json";
            if (!System.IO.File.Exists(filename))
            {
                Growl.ErrorGlobal($"没有找到配方文件{parameterName}");
                return;
            }

            var jsonData = SerializeHelper.Deserialize<AutoParameterSaveEntity>(filename);
            AutoParameterModel data = DeepCopyHelper.JsonClone( AutoParameterModelManager.Instance.AutoParameterModel);
            data.GetSaveEntity(jsonData, false);
            var result = await data.WriteToPlcAsync();
            if (result)
            {
                Growl.SuccessGlobal("设备参数切换成功");
            }
            else
            {
                Growl.ErrorGlobal("设备参数切换失败,请检查设备连接");
            }

            var weakModel =
                data.ToAutoParameterWeakRefEntity(parameterName);
            WeakReferenceMessenger.Default.Send(weakModel);
            oldParameterName = parameterName;
            Growl.Success("自动切换配方完成");
        }
    }
}