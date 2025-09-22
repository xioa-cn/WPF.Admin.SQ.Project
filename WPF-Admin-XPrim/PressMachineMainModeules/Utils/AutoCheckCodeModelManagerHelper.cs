using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace PressMachineMainModeules.Utils {
    public static class AutoCheckCodeModelManagerHelper {
        public static async Task<AutoRolaCodeType> CheckoutWorkwearCode(this AutoCheckCodeModelManager manager,
            string code) {
            if (!manager.AutoCheckCodeModel.OpenWorkwearChangeParameter)
            {
                return AutoRolaCodeType.NotOpenRolaCode;
            }

            foreach (var workwearRole in from item in manager.AutoWorkwearRolaCodeEntities
                     where item.AutoRolaCodeType == AutoRolaCodeType.Workwear
                     select item.RolaString.Split(ApplicationConfigConst.AutoModeJoinChar)
                     into workwearRole
                     where workwearRole.Length == 2
                     where code == workwearRole[0]
                     select workwearRole)
            {
                await AutoModeAutoChangeParameterHelper.ChangeParameterGlobal(workwearRole[1]);
                return AutoRolaCodeType.Workwear;
            }

            return AutoRolaCodeType.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="code"></param>
        /// <returns>条码类型 对应的AutoMode AutoMode的Step</returns>
        public static Task<(AutoRolaCodeType, string, int)>
            CheckoutMainCode(this AutoCheckCodeModelManager manager, string code) {
            if (!manager.AutoCheckCodeModel.OpenCheckMainCode)
            {
                return Task.FromResult((AutoRolaCodeType.NotOpenRolaCode, "", -1));
            }

            var list = manager.AutoMainPartialRolaCodeEntities;
            foreach (var item in list)
            {
                foreach (var mainRole in item.MainPartialRolas)
                {
                    if (mainRole.AutoRolaCodeType == AutoRolaCodeType.Main && code.Contains(mainRole.RolaString))
                    {
                        return Task.FromResult((AutoRolaCodeType.Main, item.AutoModeName, item.Step));
                    }
                }
            }


            return Task.FromResult((AutoRolaCodeType.None, "", -1));
        }

        public static Task<(AutoRolaCodeType, string, int)> CheckoutPartialCode(this AutoCheckCodeModelManager manager,
            string code) {
            if (!manager.AutoCheckCodeModel.OpenCheckMainCode)

                return Task.FromResult((AutoRolaCodeType.NotOpenRolaCode, "", -1));

            foreach (var item in manager.AutoMainPartialRolaCodeEntities)
            {
                foreach (var mainRole in item.MainPartialRolas)

                    if (mainRole.AutoRolaCodeType == AutoRolaCodeType.Partial && code.Contains(mainRole.RolaString))

                        return Task.FromResult((AutoRolaCodeType.Partial, item.AutoModeName, item.Step));
            }

            return Task.FromResult((AutoRolaCodeType.None, "", -1));
        }
    }
}