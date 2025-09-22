using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Windows;
using WPF.Admin.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Converter;
using MessageBox = System.Windows.MessageBox;

namespace PressMachineMainModeules.ViewModels;

public partial class HomeViewModel : BindableBase
{
    [ObservableProperty] private string _title = "Home";

    [ObservableProperty] private string? _ParmsNameNow;

    #region 数据结构(曲线界面数据)

    [ObservableProperty] private PloModelControlDt? ploModelControlDt10 = new();
    [ObservableProperty] private PloModelControlDt? ploModelControlDt20 = new();
    [ObservableProperty] private PloModelControlDt? ploModelControlDt30 = new();
    [ObservableProperty] private PloModelControlDt? ploModelControlDt40 = new();
    [ObservableProperty] private PloModelControlDt? ploModelControlDt50 = new();

    #endregion

    /// <summary>
    /// 左一
    /// </summary>
    public PlotViewModel PlotViewModel01 { get; set; } = new PlotViewModel(
        "DB6.0.1", "DB6.12", "DB6.16", "DB6.10", "1", "Press#1-1", PlcConnect.PlcS702);
    /// <summary>
    /// 右一
    /// </summary>
    public PlotViewModel PlotViewModel02 { get; set; } = new PlotViewModel(
        "DB6.0.2", "DB6.42", "DB6.46", "DB6.40", "2", "Press#2-1", PlcConnect.PlcS702);
    /// <summary>
    /// 左二
    /// </summary>
    public PlotViewModel PlotViewModel03 { get; set; } = new PlotViewModel(
        "DB6.0.1", "DB6.12", "DB6.16", "DB6.10", "3", "Press#1-2", PlcConnect.PlcS702);
    /// <summary>
    /// 右二
    /// </summary>
    public PlotViewModel PlotViewModel04 { get; set; } = new PlotViewModel(
        "DB6.0.2", "DB6.42", "DB6.46", "DB6.40", "4", "Press#2-2", PlcConnect.PlcS702);

    /// <summary>
    /// 三号电缸
    /// </summary>
    public PlotViewModel PlotViewModel05 { get; set; } = new PlotViewModel(
        "DB4.12.0", "DB4.2", "DB4.6", "DB4.14.0", "4", "Press#3-1", PlcConnect.PlcS703);

    public PressMachineParamsViewModel PressMachineParamsViewModel { get; set; } = new PressMachineParamsViewModel();

    private System.Windows.Threading.DispatcherTimer? _timer;

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public HomeViewModel()
    {
        // WeakReferenceMessenger.Default.Register<PressMachineStartDrawingNoWeak>(this,
        //     FromPLCStartDrwaingPressMachine); //开始画曲线

        WeakReferenceMessenger.Default.Register<PressMachieChangedOxyPlotViewWeak>(this,
            ChangedOxyPlotCanvasView); // 切换oxyplot 画布
        //this.Init();

        // // 可选：启动定时器以模拟实时数据
        // _timer = new System.Windows.Threading.DispatcherTimer {
        //     Interval = TimeSpan.FromMilliseconds(200)
        // };
        // _timer.Tick += Timer_Tick;
        // _timer.Start();
        Startup();
    }

    #region 配方本地使用读取

    public void Init()
    {
        var ConfigPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "PressConfig.json");

        var PressMachineParam = SerializeHelper.Deserialize<PressMachineHistoryParams>(ConfigPath);


        var paramsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Paramster",
            $"{PressMachineParam.ParasName}.json");

        if (!System.IO.File.Exists(paramsPath))
        {
            Growl.ErrorGlobal($"配方文件:{PressMachineParam.ParasName}不存在");
            return;
        }


        var CurrentRecipeDetail = SerializeHelper.Deserialize<PressMachineCoreParamsDa>(paramsPath);
        PressMachieChangedOxyPlotViewWeak dto = new PressMachieChangedOxyPlotViewWeak()
        {
            Token = "",
            PressMachineCoreParamsDa = CurrentRecipeDetail,
            RecipeName = PressMachineParam.ParasName,
        };

        // 软件打开时是否执行一次参数写入操作
        if (PressMachineParam.ParasWriteIndex)
        {
            //CurrentRecipeDetail.Write();
        }

        //曲线界面oxyplot画布改变
        WeakReferenceMessenger.Default.Send(dto);
    }

    #endregion

    private double _x = 0;

    private void Timer_Tick(object sender, EventArgs e)
    {
        var random = new Random();
        _x += 0.2;

        var _y1 = Math.Sin(_x) * 5 + random.NextDouble();
        var _y2 = Math.Cos(_x) * 4 + random.NextDouble();
        var _y3 = Math.Sin(_x * 0.5) * 2 + random.NextDouble();
        var _y4 = Math.Tan(_x * 0.5) + random.NextDouble();

        DispatcherHelper.CheckBeginInvokeOnUI(() =>
        {
            this.PloModelControlDt10.SetNowValue((float)_x, (float)_y1);
            this.PloModelControlDt20.SetNowValue((float)_x, (float)_y2);
            this.PloModelControlDt30.SetNowValue((float)_x, (float)_y3);
            this.PloModelControlDt40.SetNowValue((float)_x, (float)_y4);
        });

        // 为每个图表添加新点
        PlotViewModel01.AddNewPoint(_x, _y1);
        PlotViewModel02.AddNewPoint(_x, _y2);
        PlotViewModel03.AddNewPoint(_x, _y3);
        PlotViewModel04.AddNewPoint(_x, _y4);
    }

    #region 画曲线启动

    private void FromPLCStartDrwaingPressMachine(object recipient, PressMachineStartDrawingNoWeak message)
    {
        // switch (message.Token)
        // {
        //     case "10":
        //         if (PlotViewModel01!.IsUsing)
        //             Task.Factory.StartNew(PlotViewModel01!.Test, TaskCreationOptions.LongRunning);
        //         break;
        //     case "20":
        //         if (PlotViewModel02!.IsUsing)
        //             Task.Factory.StartNew(PlotViewModel02!.Test, TaskCreationOptions.LongRunning);
        //         break;
        //     case "30":
        //         if (PlotViewModel03!.IsUsing)
        //             Task.Factory.StartNew(PlotViewModel03!.Test, TaskCreationOptions.LongRunning);
        //         break;
        //     default: break;
        // }
    }

    #endregion


    #region 切判断框

    private void ChangedOxyPlotCanvasView(object recipient, PressMachieChangedOxyPlotViewWeak message)
    {
        ParmsNameNow = message.RecipeName;
        PlotViewModel01!.RecipeName = message.RecipeName;
        PlotViewModel01?.ChangeParamOxyPlotView(message.PressMachineCoreParamsDa.CurvePara!,
            message.PressMachineCoreParamsDa.MonitorRecs01!, message.RecipeName!);

        PlotViewModel02!.RecipeName = message.RecipeName;
        PlotViewModel02?.ChangeParamOxyPlotView(message.PressMachineCoreParamsDa.CurvePara!,
            message.PressMachineCoreParamsDa.MonitorRecs02!, message.RecipeName!);

        PlotViewModel03!.RecipeName = message.RecipeName;
        PlotViewModel03?.ChangeParamOxyPlotView(message.PressMachineCoreParamsDa.CurvePara!,
            message.PressMachineCoreParamsDa.MonitorRecs03!, message.RecipeName!);

        PlotViewModel04!.RecipeName = message.RecipeName;
        PlotViewModel04?.ChangeParamOxyPlotView(message.PressMachineCoreParamsDa.CurvePara!,
            message.PressMachineCoreParamsDa.MonitorRecs04!, message.RecipeName!);

        PlotViewModel05!.RecipeName = message.RecipeName;
        PlotViewModel05?.ChangeParamOxyPlotView(message.PressMachineCoreParamsDa.CurvePara!,
            message.PressMachineCoreParamsDa.MonitorRecs05!, message.RecipeName!);
    }

    #endregion
}