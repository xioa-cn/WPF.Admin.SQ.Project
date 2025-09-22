using HandyControl.Controls;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.Utils
{
    public class AutoParameterModelManager
    {
        private AutoParameterModel? autoParamerterMode;
        public AutoParameterModel AutoParameterModel
        {
            get
            {
                if (autoParamerterMode == null)
                {
                    autoParamerterMode = new AutoParameterModel();
                }
                return autoParamerterMode;
            }
        }
        public static AutoParameterModelManager Instance { get; } = new AutoParameterModelManager();
        public void Initialized()
        {
            try
            {
                autoParamerterMode = AutoParametersExcelReader.ReadExcel(
                                                ConfigPlcs.ConfigPath,
                                                        "Parameters");
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal("自动参数配置文件读取失败，请检查配置文件是否存在或格式是否正确。");
                XLogGlobal.Logger?.LogError(ex.Message, ex);
            }

        }
    }
}
