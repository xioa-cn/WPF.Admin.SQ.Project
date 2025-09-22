using CommunityToolkit.Mvvm.ComponentModel;
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using WPF.Admin.Models;
using WPF.Admin.Service.Logger;

namespace PressMachineMainModeules.Models
{
    public partial class IOPointPositionModel : BindableBase
    {
        [ObservableProperty] private bool _State;
        public string Point { get; set; }
        public string Desc { get; set; }
        public string PlcName { get; set; }
    }

    public class IOManager
    {
        private ObservableCollection<IOPointPositionModel>? _ioPointPositions;

        public ObservableCollection<IOPointPositionModel> Instance
        {
            get
            {
                if (_ioPointPositions == null)
                {
                    _ioPointPositions = new ObservableCollection<IOPointPositionModel>();
                }
                return _ioPointPositions;
            }
        }

        public static IOManager ManualIOInstance { get; } = new IOManager();
        public static IOManager IOInstance { get; } = new IOManager();

        public void Initialized(string file = null,string sheetName = "IO")
        {
            if (_ioPointPositions == null)
            {
                _ioPointPositions = new ObservableCollection<IOPointPositionModel>();
            }
            else
            {
                _ioPointPositions.Clear();
            }
            try
            {
                var ioPointPositions = IOExcelReader.ReadExcel(file, sheetName);
                foreach (var item in ioPointPositions)
                {
                    _ioPointPositions.Add(item);
                }
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError(ex.Message, ex);
            }

        }
    }
}
