using PressMachineMainModeules.Utils;

namespace PressMachineMainModeules.Models {
    public class AutoMesConfigModel {
        /// <summary>
        /// 动作前 数据请求开关
        /// </summary>
        public bool OpenBeforeChecked { get; set; }
        public AutoMesHttpMode? BeforeAutoMesHttp { get; set; }
        /// <summary>
        /// 动作完成后 数据上传开关
        /// </summary>
        public bool OpenAfterChecked { get; set; }

        /// <summary>
        /// 数据上传模式
        /// </summary>
        public AutoMesAfterMode AfterAutoMesMode { get; set; } = AutoMesAfterMode.None;
        public AutoMesHttpMode? AfterAutoMesHttp { get; set; }
        public AutoMesSqlMode? AfterAutoMesSql { get; set; }
    }

    public class AutoMesConfigManager {
        private AutoMesConfigModel? _autoMesConfig;

        public AutoMesConfigModel AutoMesConfig {
            get { return _autoMesConfig ??= LoadedMesConfig(); }
            set { _autoMesConfig = value; }
        }

        public static AutoMesConfigManager Instance { get; } = new AutoMesConfigManager();

        public void Initialized() {
            AutoMesConfig = LoadedMesConfig();
        }
        
        
        private AutoMesConfigModel LoadedMesConfig() {
            return MesConfigExcelReader.ReadExcel(sheetName: "MesConfig");
        }
    }
}