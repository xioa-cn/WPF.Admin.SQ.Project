using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.Utils
{
    public static class AutoCodeHelper
    {
        private static async Task<bool> RequestMes(AutoMesProperties autoMesProperties)
        {
            if (!AutoMesConfigManager.Instance.AutoMesConfig.OpenBeforeChecked)
            {
                ConfigPlcs.Instance[ConfigCode.Instance.PlcName]
                    .Write(ConfigCode.Instance.CodeResult, (ushort)1);
                return true;
            }

            var result =
                await AutoMesConfigManager.Instance.AutoMesConfig.BeforeAutoMesHttp
                    ?.RequestBodyBoolResult(autoMesProperties);
            if (result)
            {
                ConfigPlcs.Instance[ConfigCode.Instance.PlcName]
                    .Write(ConfigCode.Instance.CodeResult, (ushort)1);
                return true;
            }

            ConfigPlcs.Instance[ConfigCode.Instance.PlcName]
                .Write(ConfigCode.Instance.CodeResult, (ushort)0);
            return false;
        }

        /// <summary>
        /// 零件码组赋值
        /// </summary>
        /// <param name="autoPartialCodeModel"></param>
        /// <param name="code"></param>
        /// <param name="autoModeName"></param>
        /// <param name="step"></param>
        private static void AutoSetPartialCode(AutoCodeModel? autoPartialCodeModel, string code,
            string? autoModeName, int step)
        {
            if (autoPartialCodeModel is null) return;
            if (!AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.OpenCheckPartialCode) return;
            var nullIndex = 0;
            var findAutoPartial = autoPartialCodeModel.PartialCodes.FirstOrDefault(x =>
                x.AutoMode == autoModeName);
            var partialRolaList = findAutoPartial?.PartialCodes[step];
            if (partialRolaList == null)
            {
                return;
            }

            foreach (var partialRola in partialRolaList)
            {
                if (string.IsNullOrEmpty(partialRola))
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { partialRolaList[index: nullIndex] = code; });
                    break;
                }

                nullIndex++;
            }

            foreach (var item in partialRolaList)
            {
                if (string.IsNullOrEmpty(item))
                {
                    return;
                }
            }
            // TODO 写零件码完成信号给PLC
        }

        /// <summary>
        /// 检查条码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="autoMesProperties"></param>
        /// <param name="autoPartialCodeModel"></param>
        /// <returns></returns>
        public static async Task<(bool, AutoRolaCodeType)> CheckCode(string code, AutoMesProperties autoMesProperties,
            AutoCodeModel? autoPartialCodeModel = null)
        {
            var checkTemp = await AutoCheckCodeModelManager.Instance.CheckoutWorkwearCode(code);
            if (checkTemp == AutoRolaCodeType.Workwear)
            {
                return (true, checkTemp);
            }

            var isMainCode = await AutoCheckCodeModelManager.Instance.CheckoutMainCode(code);
            if (isMainCode.Item1 == AutoRolaCodeType.Main || isMainCode.Item1 == AutoRolaCodeType.NotOpenRolaCode)
            {
                AutoSetMainCode(autoPartialCodeModel, code, isMainCode.Item2, isMainCode.Item3);
                return (await RequestMes(autoMesProperties), checkTemp);
            }

            var isPartialCode = await AutoCheckCodeModelManager.Instance.CheckoutPartialCode(code);

            if (isPartialCode.Item1 == AutoRolaCodeType.Partial)
            {
                AutoSetPartialCode(autoPartialCodeModel, code, isPartialCode.Item2, isPartialCode.Item3);
                return (true, isPartialCode.Item1);
            }

            return (false, checkTemp);
        }

        private static void AutoSetMainCode(AutoCodeModel? autoPartialCodeModel, string code,
            string autoModeName, int step)
        {
            if (autoPartialCodeModel is null) return;
            //var nullIndex = 0;
            var content = autoPartialCodeModel.PartialCodes.FirstOrDefault(x => x.AutoMode == autoModeName);
            if (content is null) return;
            DispatcherHelper.CheckBeginInvokeOnUI(() => { content.MainCode[step] = code; });
            // if (rolaList != null)
            // {
            //     foreach (var partialRola in rolaList)
            //     {
            //         if (string.IsNullOrEmpty(partialRola))
            //         {
            //             DispatcherHelper.CheckBeginInvokeOnUI(() => { rolaList[index: nullIndex] = code;
            //             break;
            //         }
            //
            //         nullIndex++;
            //     }
            // }
        }
    }
}