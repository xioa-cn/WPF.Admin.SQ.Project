
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models
{
    public partial class AlarmPositionModel : BindableBase
    {
        public bool State { get; set; }
        public string Desc { get; set; }
        public string Position { get; set; }
        public string Db { get; set; }
        public string PlcName { get; set; }

        public string getFullPosition
        {
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

    public class AlarmManager
    {
        private ObservableCollection<AlarmPositionModel>? _alarmPositions;
        public ObservableCollection<AlarmPositionModel> AlarmPositions
        {
            get
            {
                if (_alarmPositions == null)
                {
                    _alarmPositions = new ObservableCollection<AlarmPositionModel>();
                }
                return _alarmPositions;
            }
        }

        public static AlarmManager Instance { get; } = new AlarmManager();

        public void Initialized(string? file = null,string sheetName = "alarm")
        {
            if (_alarmPositions == null)
            {
                _alarmPositions = new ObservableCollection<AlarmPositionModel>();
            }
            else
            {
                _alarmPositions.Clear();
            }
            try
            {
                // Implement logic to read from the specified file and populate _manualParameters
                // This is a placeholder for the actual implementation
                // Example: _manualParameters = ReadFromFile(file);
                AlarmExcelReader.ReadExcel(file, sheetName).ToList().ForEach(item => _alarmPositions.Add(item));
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error
                Console.WriteLine($"Error initializing ManualParametersManager: {ex.Message}");
            }
        }
    }
}
