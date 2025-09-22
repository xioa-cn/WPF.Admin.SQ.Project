using PressMachineMainModeules.Config;
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models {
    public partial class HomePositionModel : BindableBase {
        [Description("名称 AutoMode")] public string Desc { get; set; }
        [Description("启停信号")] public string StartPosition { get; set; }
        [Description("位移")] public string PositionPosition { get; set; }
        [Description("压力")] public string PressPosition { get; set; }
        [Description("结果")] public string ResultPosition { get; set; }
        [Description("序号")] public string KeyNum { get; set; }
        [Description("PLC序号")] public string PlcName { get; set; }
        [Description("最终位移")] public string? MaxPosition { get; set; }
        [Description("最大压力")] public string? MaxPress { get; set; }
        [Description("最终压力")] public string? EndPress { get; set; }
        [Description("OK数量")] public string? OKSum { get; set; }
        [Description("NG数量")] public string? NGSum { get; set; }
        [Description("附加1")] public string? Add1 { get; set; }
        [Description("附加2")] public string? Add2 { get; set; }
        [Description("附加3")] public string? Add3 { get; set; }
        [Description("是否启动步序")] public bool StartStep { get; set; }
        [Description("步序")] public string Step { get; set; }

        [Description("步序总数")] public int StepSum { get; set; }
        
        [Description("配方号")] public string FormulaNum { get; set; }
        
        public bool United { get; set; }

        public string UnitedValue { get; set; }
    }

    public class HomeManager {
        private ObservableCollection<HomePositionModel>? _homePositionModels;

        public ObservableCollection<HomePositionModel> HomePositionModels {
            get
            {
                if (_homePositionModels == null)
                {
                    _homePositionModels = new ObservableCollection<HomePositionModel>();
                }

                return _homePositionModels;
            }
        }

        public static HomeManager Instance { get; } = new HomeManager();

        public void Initiailzed() {
            if (_homePositionModels == null)
            {
                _homePositionModels = new ObservableCollection<HomePositionModel>();
            }
            else
            {
                _homePositionModels.Clear();
            }

            try
            {
                // Implement logic to read from the specified file and populate _manualParameters
                // This is a placeholder for the actual implementation
                // Example: _manualParameters = ReadFromFile(file);
                HomeExcelReader.ReadExcel(ConfigPlcs.ConfigPath, "AutoMode").ToList()
                    .ForEach(item => _homePositionModels.Add(item));
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                Console.WriteLine($"Error initializing ManualParametersManager: {ex.Message}");
            }
        }
    }
}