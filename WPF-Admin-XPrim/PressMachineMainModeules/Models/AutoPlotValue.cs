using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models {
    public partial class AutoPlotValue : ParameterBase {
        [ObservableProperty] private string _Name;

        private string dbPoint;

        public string DbPoint {
            get => dbPoint;
            set
            {
                var valueList = value.Split('-');
                dbPoint = valueList[0];
                if (valueList.Length > 1)
                {
                    Name = valueList[1];
                }

                if (valueList.Length > 2)
                {
                    Unit = valueList[2];
                }

                if (valueList.Length > 3)
                {
                    Type = valueList[3];
                }
            }
        }

        private float _value = 0.00f;
        public string PlcName { get; set; }
        [ObservableProperty] private string _Unit;
    }
}