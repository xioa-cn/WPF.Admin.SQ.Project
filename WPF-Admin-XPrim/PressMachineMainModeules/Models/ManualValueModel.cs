
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;

namespace PressMachineMainModeules.Models
{
    public class ManualValueModel : ManualParametersModel
    {
    }

    public class ManualValueManager
    {
        private ObservableCollection<ManualValueModel>? _ManualValue;
        public ObservableCollection<ManualValueModel>? ManualValue
        {
            get
            {
                if (_ManualValue == null)
                {
                    _ManualValue = new ObservableCollection<ManualValueModel>();
                }
                return _ManualValue;
            }
        }

        public static ManualValueManager Instance { get; } = new ManualValueManager();

        public void Initialized(string file = null, string sheetName = "ManualValue")
        {
            if (_ManualValue == null)
            {
                _ManualValue = new ObservableCollection<ManualValueModel>();
            }
            else
            {
                _ManualValue.Clear();
            }
            try
            {
                // Implement logic to read from the specified file and populate _manualParameters
                // This is a placeholder for the actual implementation
                // Example: _manualParameters = ReadFromFile(file);
                ManualParameterExcelReader.ReadExcel(file, sheetName).Select(item=>
                    new ManualValueModel
                    {
                        Desc = item.Desc,
                        Db = item.Db,
                        Position = item.Position,
                        Type = item.Type,
                        Unit = item.Unit,
                        PlcName = item.PlcName,
                        Multiplier = item.Multiplier,
                        Min = item.Min,
                        Max = item.Max,
                        Calculation = item.Calculation
                    }
                ).ToList().ForEach(item => _ManualValue.Add(item));
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                Console.WriteLine($"Error initializing ManualParametersManager: {ex.Message}");
            }
        }
    }

}
