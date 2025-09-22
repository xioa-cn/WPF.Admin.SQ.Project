using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PressMachineMainModeules.Models {
    public class ManualPointPositionModel : ParameterBase {
        [Description("DB块")] public string Db { get; set; }
        [Description("偏移量")] public string Position { get; set; }
        [Description("写入状态")] public string WriteState { get; set; }
        [Description("写入值")] public string WValue { get; set; }
        public string getFullPosition {
            get
            {
                if (string.IsNullOrEmpty(Position))
                {
                    return $"{Db}";
                }

                return $"{Db}.{Position}";
            }
        }
        public string PlcName { get; set; }
    }

    public class ManualButtonManager {
        private static ObservableCollection<ManualPointPositionModel>? manualPointPositionModels;

        public static ObservableCollection<ManualPointPositionModel> Instance {
            get
            {
                if (manualPointPositionModels == null)
                {
                    manualPointPositionModels = new ObservableCollection<ManualPointPositionModel>();
                }

                return manualPointPositionModels;
            }
        }

        public static void Initialized(string file = null, string sheetName = "ManualButton") {
            if (manualPointPositionModels == null)
            {
                manualPointPositionModels = new ObservableCollection<ManualPointPositionModel>();
            }
            else
            {
                manualPointPositionModels.Clear();
            }

            try
            {
                // Implement logic to read from the specified file and populate manualPointPositionModels
                // This is a placeholder for the actual implementation
                // Example: manualPointPositionModels = ReadFromFile(file);
                ManualButtonExcelReader.ReadExcel(file, sheetName).ToList()
                    .ForEach(item => manualPointPositionModels.Add(item));
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                Console.WriteLine($"Error initializing ManualButtonManager: {ex.Message}");
            }
        }
    }
}