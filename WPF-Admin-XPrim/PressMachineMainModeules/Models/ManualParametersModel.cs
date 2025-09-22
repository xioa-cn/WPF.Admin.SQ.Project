using CommunityToolkit.Mvvm.ComponentModel;
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models {
    public partial class ManualParametersModel : ParameterBase {
        public string Db { get; set; }
        public string Position { get; set; }
        public string Unit { get; set; }
        public string PlcName { get; set; }
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
    }

    public class ManualParametersManager {
        private ObservableCollection<ManualParametersModel>? _manualParameters;

        public ObservableCollection<ManualParametersModel>? ManualParameters {
            get
            {
                if (_manualParameters == null)
                {
                    _manualParameters = new ObservableCollection<ManualParametersModel>();
                }

                return _manualParameters;
            }
        }

        public static ManualParametersManager Instance { get; } = new ManualParametersManager();

        public void Initialized(string file = null, string sheetName = "ManualParameters") {
            if (_manualParameters == null)
            {
                _manualParameters = new ObservableCollection<ManualParametersModel>();
            }
            else
            {
                _manualParameters.Clear();
            }

            try
            {
                // Implement logic to read from the specified file and populate _manualParameters
                // This is a placeholder for the actual implementation
                // Example: _manualParameters = ReadFromFile(file);
                ManualParameterExcelReader.ReadExcel(file, sheetName).ToList()
                    .ForEach(item => _manualParameters.Add(item));
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                Console.WriteLine($"Error initializing ManualParametersManager: {ex.Message}");
            }
        }
    }
}