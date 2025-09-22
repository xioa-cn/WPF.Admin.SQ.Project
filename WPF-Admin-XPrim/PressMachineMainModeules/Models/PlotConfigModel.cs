using System.Collections.ObjectModel;
using PressMachineMainModeules.Utils;

namespace PressMachineMainModeules.Models {
    public class PlotConfigModel(string autoModeName) {
        public string AutoModeName { get; set; } = autoModeName;
        public string? XName { get; set; }
        public string? XUnit { get; set; }
        public string? YName { get; set; }
        public string? YUnit { get; set; }
        public bool IsZoom { get; set; }
        public bool IsPan { get; set; }
        public string? PlotColor { get; set; }
    }


    public class PlotConfigManager {
        private List<PlotConfigModel>? _plotConfigModels;

        public List<PlotConfigModel> PlotConfigModels {
            get
            {
                if (_plotConfigModels == null)
                {
                    _plotConfigModels = new List<PlotConfigModel>();
                }

                return _plotConfigModels;
            }
        }

        public PlotConfigModel? this[string? key] {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }
                var find = PlotConfigModels.FirstOrDefault(item => item.AutoModeName == key);
                return find;
            }
        }

        public static PlotConfigManager Instance { get; } = new PlotConfigManager();

        public void Initialized(string? file = null, string sheetName = "PlotConfig") {
            if (_plotConfigModels == null)
            {
                _plotConfigModels = new List<PlotConfigModel>();
            }
            else
            {
                _plotConfigModels.Clear();
            }

            try
            {
                PlotConfigExcelReader.ReadExcel(file, sheetName).ToList().ForEach(item => _plotConfigModels.Add(item));
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                Console.WriteLine($"Error initializing ManualParametersManager: {ex.Message}");
            }
        }
    }
}