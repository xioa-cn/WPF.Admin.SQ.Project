using CommunityToolkit.Mvvm.ComponentModel;
using PressMachineMainModeules.Attributes;
using PressMachineMainModeules.Config;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Admin.Models;

namespace PressMachineMainModeules.Models
{
    /// <summary>
    /// 压机配方
    /// </summary>
    [Serializable]
    public partial class PressMachineCoreParamsDa : BindableBase
    {
        [BasePlc(nameof(PlcConnect.PlcS702))]
        [ObservableProperty] private float _左待机位置 = 1;
        [BasePlc(nameof(PlcConnect.PlcS702))]
        [ObservableProperty] private float _左待机速度 = 1;
        [BasePlc(nameof(PlcConnect.PlcS702))]
        [ObservableProperty] private float _右待机位置 = 2;
        [BasePlc(nameof(PlcConnect.PlcS702))]
        [ObservableProperty] private float _右待机速度 = 2;

        [BasePlc(nameof(PlcConnect.PlcS703))]
        [ObservableProperty] private float _Press3待机位置 = 3;
        [BasePlc(nameof(PlcConnect.PlcS703))]
        [ObservableProperty] private float _Press3待机速度 = 3;
        [BasePlc(nameof(PlcConnect.PlcS703))]
        [ObservableProperty] private float _Press4待机位置 = 4;
        [BasePlc(nameof(PlcConnect.PlcS703))]
        [ObservableProperty] private float _Press4待机速度 = 4;

        /// <summary>
        /// X轴参数
        /// </summary>
        [ObservableProperty] private PressMachineParamsXDa _PlcParamsX = new PressMachineParamsXDa();

        // 电缸参数
        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS702))] 
        private PressMachineParamsDa _PlcParams01 = new PressMachineParamsDa();

        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS702))] 
        private PressMachineParamsDa _PlcParams02 = new PressMachineParamsDa();

        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS702))] 
        private PressMachineParamsDa _PlcParams03 = new PressMachineParamsDa();

        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS702))] 
        private PressMachineParamsDa _PlcParams04 = new PressMachineParamsDa();

        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS703))] 
        private PressMachineParamsDa _Press3Params01 = new PressMachineParamsDa();

        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS703))] 
        private PressMachineParamsDa _Press3Params02 = new PressMachineParamsDa();

        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS703))] 
        private PressMachineParamsDa _Press3Params03 = new PressMachineParamsDa();

        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS703))] 
        private PressMachineParamsDa _Press3Params04 = new PressMachineParamsDa();

        /// <summary>
        /// 滑台
        /// </summary>
        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS701))]
        private PressMachineSlipwayDa _SlipwayDa = new PressMachineSlipwayDa();

        /// <summary>
        /// Press1横移
        /// </summary>
        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS701))] 
        private PressMachineSideswayDa _SideswayDaLeft = new PressMachineSideswayDa();

        /// <summary>
        /// Press2横移
        /// </summary>
        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS701))]
        private PressMachineSideswayDa _SideswayDaRight = new PressMachineSideswayDa();

        /// <summary>
        /// Press3横移
        /// </summary>
        [ObservableProperty]
        [BasePlc(nameof(PlcConnect.PlcS701))]
        private PressMachineSideswayDa _SideswayDaCenter = new PressMachineSideswayDa();

        [Column("CurvePara")] public CurvePara CurvePara { get; set; } = new();
        [Column("MonitorRecs1")] public ObservableCollection<MonitorRec> MonitorRecs01 { get; set; } = new();
        [Column("MonitorRecs2")] public ObservableCollection<MonitorRec> MonitorRecs02 { get; set; } = new();
        [Column("MonitorRecs3")] public ObservableCollection<MonitorRec> MonitorRecs03 { get; set; } = new();
        [Column("MonitorRecs4")] public ObservableCollection<MonitorRec> MonitorRecs04 { get; set; } = new();
        [Column("MonitorRecs5")] public ObservableCollection<MonitorRec> MonitorRecs05 { get; set; } = new();
    }
}