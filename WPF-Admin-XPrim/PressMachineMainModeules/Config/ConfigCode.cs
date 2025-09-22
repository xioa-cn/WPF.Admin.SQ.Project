using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;

namespace PressMachineMainModeules.Config
{
    public enum CodeModeEnum
    {
        None,
        ComCode,
        PlcCode,
    }

    public class ConfigCode
    {
        public CodeModeEnum CodeMode { get; set; } = CodeModeEnum.None;

        private static ConfigCode? _instance;

        public string CodeResult { get; set; }

        public string PlcName { get; set; }

        private ConfigInformationModel? _barcodePlagiarismCheck;

        /// <summary>
        /// 条码查重
        /// </summary>
        public ConfigInformationModel BarcodePlagiarismCheck
        {
            get { return _barcodePlagiarismCheck ??= new ConfigInformationModel(); }
            set { _barcodePlagiarismCheck = value; }
        }

        private ConfigInformationModel? _rework;

        /// <summary>
        /// 返修
        /// </summary>
        public ConfigInformationModel Rework
        {
            get { return _rework ??= new ConfigInformationModel(); }
            set { _rework = value; }
        }

        public static ConfigCode Instance
        {
            get { return _instance ??= ConfigCodeExcelReader.ReadExcel(ConfigPlcs.ConfigPath, "Code"); }
        }

        public string Parameter { get; set; }
    }
}