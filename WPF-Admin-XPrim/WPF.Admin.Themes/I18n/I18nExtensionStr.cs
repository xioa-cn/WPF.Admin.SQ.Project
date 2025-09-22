using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Themes.I18n
{
    public static class I18nExtensionStr
    {
        /// <summary>
        /// 拓展翻译
        /// </summary>
        public static void I18nUseExtensionFunc()
        {
            I18nManager.Instance.ExtensionValueFunc(I18nExtensionStr.ExtensionStr);
        }

        public static string ExtensionStr(string key)
        {
            var keyArr = key.Split('.');
            if (keyArr.Length == 2)
            {
                if (PressMachineExcelConfigHeader.Contains(keyArr[0]))
                {
                    return keyArr[1];
                }
            }

            return key switch
            {
                "AppBaseOnName" => ApplicationConfigConst.InstanceApplicationGlobalName.AppBaseOnName,
                "AppCompanyName" => ApplicationConfigConst.InstanceApplicationGlobalName.AllStringName,
                "AppName" => AppSettings.Default!.AppName,
                _ => $"[{key}]"
            };
        }

        private static string[]? _pressMachineExcelConfigHeader;

        public static string[] PressMachineExcelConfigHeader
        {
            get
            {
                return _pressMachineExcelConfigHeader ??= new[]
                {
                    "ManualBtn", "ManualIo", "ManualNumerical", "ManualParameter", "ParametersHeader",
                    "UserParameters", "PlotHeader"
                };
            }
        }
    }
}