using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Controls;
using HslCommunication.Core.Device;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Utils;
using PressMachineMainModeules.ViewModels;
using WPF.Admin.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.Models {
    public partial class PlotUtils : BindableBase {

        public bool United { get; set; }

        public string UnitedValue { get; set; }
        
        public int Key { get; set; }

        public string Name { get; set; }

        public string Startup { get; set; }

        public string PlcName { get; set; }

        public HomePositionModel PlotInfo { get; set; }


        public bool StartStep { get; set; }

        private string _step;
        [ObservableProperty] private bool _isShowCode = true;

        public OpValueType OpValueType { get; private set; }

        public PlotUtils() {
            if (ConfigCode.Instance.CodeMode == CodeModeEnum.None)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { this.IsShowCode = false; });
            }
        }

        public string Step {
            get => this._step;
            set
            {
                var valuelist = value.Split('-');
                this._step = valuelist[0];
                if (valuelist.Length > 1)
                {
                    OpValueType = OpValueTypeHelper.GetValueType(valuelist[1]);
                }
            }
        }

        public int? StepRead(DeviceCommunication plc) {
            return (int?)ParameterBaseReadHelper.Read(Step, plc, OpValueType);
        }

        public int? FormulaRead(DeviceCommunication plc) {
            if (string.IsNullOrEmpty(Plot.FormulaNum))
            {
                return null;
            }

            var useLit = Plot.FormulaNum.Split('-');
            if (useLit.Length == 2)
            {
                return (int?)ParameterBaseReadHelper.Read(useLit[0], plc, OpValueTypeHelper.GetValueType(useLit[1]));
            }
            var message =$"公式格式错误~配方序号 {Plot.PressMachineName}";
            XLogGlobal.Logger?.LogError(message);
            Growl.ErrorGlobal(message);
            return null;
        }


        [ObservableProperty] private AutoPlotViewModel _Plot;

        private Signal? _signal;

        public Signal Signal {
            get { return _signal ??= new Signal(); }
        }
    }
}