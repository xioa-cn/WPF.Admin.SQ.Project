using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Service.Utils;
using WPF.Admin.Themes.Helper;
using Edge = PressMachineMainModeules.Models.Edge;
using Signal = PressMachineMainModeules.Models.Signal;

namespace PressMachineMainModeules.ViewModels;

public partial class HomeViewModel
{
    [ObservableProperty] private string _Code = "等待扫码中";
    [ObservableProperty] private string _CodeSecond = "等待扫码中";
    private ThreadSafeQueue<string> _CodeList = new ThreadSafeQueue<string>();
    public PlotStatus PlotStatus { get; set; } = new PlotStatus();
    private readonly string _codeTemplate = "等待扫码中";

    public IOPointVal IOPointVal { get; set; } = new IOPointVal();

    private MaualShowFloatData manualFloatData = new MaualShowFloatData();

    [RelayCommand]
    private void ClearCode()
    {
        this.Code = _codeTemplate;
    }
    private PressMachineStatusManager _PressMachineStatusManager01 = new PressMachineStatusManager()
    {
        Num = 1,
        Status = new PressMachineStatus(),
    };
    private PressMachineStatusManager _PressMachineStatusManager02 = new PressMachineStatusManager()
    {
        Num = 2,
        Status = new PressMachineStatus(),
    };
    private PressMachineStatusManager _PressMachineStatusManager03 = new PressMachineStatusManager()
    {
        Num = 3,
        Status = new PressMachineStatus(),
    };
    private void Comm()
    {
        Signal signalleft1 = new Signal();
        Signal signalleft2 = new Signal();
        Signal signalright1 = new Signal();
        Signal signalright2 = new Signal();
        Signal plotend = new Signal();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        int sleepTime = 10;
        var beat = 1;
        while (true)
        {
            var connectResult = PlcConnect.LineFromPCToPLC();
            if (connectResult)
            {
                XLogGlobal.Logger?.LogInfo("PLC连接成功");
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接成功"); });
            }
            else
            {
                XLogGlobal.Logger?.LogInfo("PLC连接失败！将在五秒都重新尝试连接");
                Growl.WarningGlobal("PLC连接失败！将在五秒都重新尝试连接");
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接失败！将在五秒都重新尝试连接", 3000); });
            }

            try
            {
                while (PlcConnect.GoOn)
                {
                    //if (stopwatch.ElapsedMilliseconds >= 300)
                    //{
                    //    beat = beat == 1 ? 0 : 1;
                    //    var heatBeat = PlcConnect.Plc.Write("DB4.0", (ushort)beat);
                    //    if (!heatBeat.IsSuccess)
                    //    {
                    //        throw new Exception("PLC交互异常 心跳异常");
                    //    }

                    //    stopwatch.Restart();
                    //}





                    // 位置 压力 速度 扭矩
                    var floatContent1 = PlcConnect.Plc?.ReadDouble("MB504");
                    var floatContent2 = PlcConnect.Plc?.ReadDouble("MB520");
                    var floatContent3 = PlcConnect.Plc?.ReadDouble("MB536");

                    var floatContent4 = PlcConnect.Plc?.ReadFloat("MB548");
                    var floatContent5 = PlcConnect.Plc?.ReadFloat("MB552");
                    var floatContent6 = PlcConnect.Plc?.ReadFloat("MB556");
                    if (floatContent1 is not null && floatContent1.IsSuccess)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            //PloModelControlDt10.NowPos = (float)Math.Round(floatContent.Content[0], 2);
                            PloModelControlDt10.NowPre = (float)Math.Round(floatContent1.Content, 2);
                            PloModelControlDt10.Hz = Math.Abs((float)Math.Round(floatContent4.Content, 2));

                            //PloModelControlDt20.NowPos = (float)Math.Round(floatContent.Content[4], 2);
                            PloModelControlDt20.NowPre = (float)Math.Round(floatContent2.Content, 2);
                            PloModelControlDt20.Hz = Math.Abs((float)Math.Round(floatContent5.Content, 2));

                            //PloModelControlDt30.NowPos = (float)Math.Round(floatContent.Content[8], 2);
                            PloModelControlDt30.NowPre = (float)Math.Round(floatContent3.Content, 2);
                            PloModelControlDt30.Hz = Math.Abs((float)Math.Round(floatContent6.Content, 2));

                            manualFloatData.压力1 = PloModelControlDt10.NowPre;
                            manualFloatData.频率1 = PloModelControlDt10.Hz;
                            manualFloatData.压力2 = PloModelControlDt20.NowPre;
                            manualFloatData.频率2 = PloModelControlDt20.Hz;
                            manualFloatData.压力3 = PloModelControlDt30.NowPre;
                            manualFloatData.频率3 = PloModelControlDt30.Hz;

                            WeakReferenceMessenger.Default.Send(manualFloatData);
                            //PloModelControlDt40.NowPos = (float)Math.Round(floatContent.Content[12], 2);
                            //PloModelControlDt40.NowPre = (float)Math.Round(floatContent.Content[13], 2);
                        });
                    }
                    else
                    {
                        throw new Exception("PLC交互异常 读取位置压力失败");
                    }
                    // 启停 
                    var startBoolPoint = PlcConnect.Plc.ReadBool("MX100.0", 3);
                    if (!startBoolPoint.IsSuccess)
                    {
                        throw new Exception("PLC交互异常 读取启停信号失败");
                    }

                    var readbool10 = PlcConnect.Plc?.ReadBool("MX10.0", 6);
                    var readbool20 = PlcConnect.Plc?.ReadBool("MX20.0", 6);
                    var readbool30 = PlcConnect.Plc?.ReadBool("MX30.0", 6);
                    var readbool1000 = PlcConnect.Plc?.ReadBool("MX1000.0", 3);
                    //var readbool100 = PlcConnect.Plc?.ReadBool("")
                    if (readbool10 is not null
                        && readbool20 is not null
                        && readbool30 is not null
                        && readbool1000 is not null
                        && readbool10.IsSuccess
                        && readbool20.IsSuccess
                        && readbool30.IsSuccess
                        && readbool1000.IsSuccess)
                    {
                        _PressMachineStatusManager01.Status.自动模式 = !readbool10.Content[0];
                        _PressMachineStatusManager01.Status.手动模式 = readbool10.Content[0];

                        _PressMachineStatusManager01.Status.电缸启用 = readbool1000.Content[0];
                        _PressMachineStatusManager01.Status.电缸屏蔽 = !readbool1000.Content[0];

                        _PressMachineStatusManager01.Status.自动停止 = !startBoolPoint.Content[0];
                        _PressMachineStatusManager01.Status.自动启动 = startBoolPoint.Content[0];

                        WeakReferenceMessenger.Default.Send(_PressMachineStatusManager01);

                        _PressMachineStatusManager02.Status.自动模式 = !readbool20.Content[0];
                        _PressMachineStatusManager02.Status.手动模式 = readbool20.Content[0];

                        _PressMachineStatusManager02.Status.电缸启用 = readbool1000.Content[1];
                        _PressMachineStatusManager02.Status.电缸屏蔽 = !readbool1000.Content[1];

                        _PressMachineStatusManager02.Status.自动停止 = !startBoolPoint.Content[1];
                        _PressMachineStatusManager02.Status.自动启动 = startBoolPoint.Content[1];

                        WeakReferenceMessenger.Default.Send(_PressMachineStatusManager02);

                        _PressMachineStatusManager03.Status.自动模式 = !readbool30.Content[0];
                        _PressMachineStatusManager03.Status.手动模式 = readbool30.Content[0];

                        _PressMachineStatusManager03.Status.电缸启用 = readbool1000.Content[2];
                        _PressMachineStatusManager03.Status.电缸屏蔽 = !readbool1000.Content[2];

                        _PressMachineStatusManager03.Status.自动停止 = !startBoolPoint.Content[2];
                        _PressMachineStatusManager03.Status.自动启动 = startBoolPoint.Content[2];

                        WeakReferenceMessenger.Default.Send(_PressMachineStatusManager03);
                    }

                    //var sta = PlcConnect.Plc?.ReadBool("DB1.10", 11);
                    //var staFloat = PlcConnect.Plc?.ReadFloat("DB2.4", 7);
                    //if (sta.IsSuccess)
                    //{
                    //    IOPointVal.原点触发 = sta.Content[0];
                    //    IOPointVal.X轴原点 = sta.Content[1];
                    //    IOPointVal.自动模式 = sta.Content[2];
                    //    IOPointVal.自动状态 = sta.Content[3];
                    //    IOPointVal.压机待机位 = sta.Content[4];
                    //    IOPointVal.X轴待机位 = sta.Content[5];
                    //    IOPointVal.X轴一次待机位 = sta.Content[6];
                    //    IOPointVal.X轴压装位置一 = sta.Content[7];
                    //    IOPointVal.X轴压装位置二 = sta.Content[8];
                    //    IOPointVal.X轴压装位置三 = sta.Content[9];
                    //    IOPointVal.X轴压装位置四 = sta.Content[10];
                    //    if (staFloat.IsSuccess)
                    //    {
                    //        IOPointVal.手动速度 = staFloat.Content[0];
                    //        IOPointVal.手动保护 = staFloat.Content[1];
                    //        IOPointVal.X轴手动速度 = (float)Math.Round(staFloat.Content[2], 2);
                    //        IOPointVal.X轴位置指示 = (float)Math.Round(staFloat.Content[6], 2);
                    //    }
                    //    if (floatContent.IsSuccess)
                    //    {
                    //        IOPointVal.压机位置指示 = (float)Math.Round(floatContent.Content[0], 2);
                    //        IOPointVal.压机压力指示 = (float)Math.Round(floatContent.Content[1], 2);
                    //    }
                    //    WeakReferenceMessenger.Default.Send(IOPointVal);
                    //}
                    //else
                    //{
                    //    //throw new Exception("PLC交互异常 读取状态失败");
                    //}


                    //// 左侧 右侧 压装步骤
                    //var startNumPoint = PlcConnect.Plc.ReadInt16("DB4.10.0");
                    //if (!startNumPoint.IsSuccess)
                    //{
                    //    throw new Exception("PLC交互异常 读取压装步骤失败");
                    //}

                    signalleft1.Update(startBoolPoint.Content[0]);
                    signalleft2.Update(startBoolPoint.Content[1]);
                    signalright1.Update(startBoolPoint.Content[2]);
                    // signalright2.Update(startBoolPoint.Content );

                    if (signalleft1.State == Edge.Rising)
                    {
                        if (PlotViewModel01.IsUsing)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => { PloModelControlDt10.Result = ""; });

                            Task.Run(() => { PlotViewModel01.Test(Code); });

                            PressMachineGettingIndex.ChangeHomeShow(SetIndexKeyEnum.pre1);
                            PlotStatus.Plot01End = false; // 把结束状态置为false
                            //PlotStatus.Plot02End = false; // 把结束状态置为false
                            //PlotStatus.Plot03End = false; // 把结束状态置为false
                            //PlotStatus.Plot04End = false; // 把结束状态置为false
                        }
                        else
                        {
                            throw new Exception("左一信号异常没有收到完成信号");
                        }
                    }

                    if (signalleft2.State == Edge.Rising)
                    {
                        if (PlotViewModel02.IsUsing)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => { PloModelControlDt20.Result = ""; });
                            Task.Run(() => { PlotViewModel02.Test(Code); });

                            //PressMachineGettingIndex.ChangeHomeShow(SetIndexKeyEnum.pre2);
                        }
                        else
                        {
                            throw new Exception("左二信号异常没有收到完成信号");
                        }
                    }

                    if (signalright1.State == Edge.Rising)
                    {
                        if (PlotViewModel03.IsUsing)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => { PloModelControlDt30.Result = ""; });
                            Task.Run(() => { PlotViewModel03.Test(Code); });
                            //  PressMachineGettingIndex.ChangeHomeShow(SetIndexKeyEnum.pre3);
                        }
                        else
                        {
                            throw new Exception("右一信号异常没有收到完成信号");
                        }
                    }

                    //if (signalright2.State == Edge.Rising)
                    //{
                    //    if (PlotViewModel04.IsUsing)
                    //    {
                    //        DispatcherHelper.CheckBeginInvokeOnUI(() => { PloModelControlDt40.Result = ""; });
                    //        Task.Run(() => { PlotViewModel04.Test(Code); });
                    //        PressMachineGettingIndex.ChangeHomeShow(SetIndexKeyEnum.pre4);
                    //    }
                    //    else
                    //    {
                    //        throw new Exception("右二信号异常没有收到完成信号");
                    //    }
                    //}

                    var isPlotEndStatus = PlotStatus.GetHavingStart();
                    plotend.Update(isPlotEndStatus);
                    if (plotend.State == Edge.Rising && !string.IsNullOrWhiteSpace(this.Code))
                    {
                        if (Common.UsingCodeList)
                        {
                            if (_CodeList.Count > 0)
                            {
                                if (_CodeList.TryDequeue(out string _temp))
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() => { this.Code = _temp; });
                                    //var codeStatus = PlcConnect.Plc.Write("DB4.18.0", true);
                                }
                            }
                        }
                        else
                        {
                            ClearCode();
                            sleepTime = 10;
                        }
                    }

                    if (plotend.State == Edge.Falling)
                    {
                        sleepTime = 500;
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            SnackbarHelper.Show("画曲线状态:将抑制状态采集以加速曲线采集", 3000);
                        });
                    }

                    if (isPlotEndStatus && sleepTime != 500)
                    {
                        sleepTime = 500;
                    }

                    #region 报警
                    // 报警
                    if (false)
                    {
                        var alarm = PlcConnect.Plc.ReadBool("DB5.0", 32);
                        var al = PlcConnect.Plc.Read("DB5.0.0", 27);
                        var dalarm = PlcConnect.Plc.ReadBool("M2000.5");
                        if (alarm.IsSuccess && dalarm.IsSuccess)
                        {
                            var status = alarm.Content;

                            var status1 = dalarm.Content;
                            var alarmList = new List<(string, bool?)>
                        {
                            ("急停按钮按下", status[8]),
                            ("屏蔽门未关闭", status[9]),
                            ("压机使能未就绪", status[10]),
                            ("X轴使能未就绪", status[11]),
                            ("压机上限位报警", status[12]),
                            ("压机下限位报警", status[13]),
                            ("X轴左限位报警", status[14]),
                            ("X轴右限位报警", status[15]),

                            ("压机轴报错", status[0]),
                            ("X轴报错", status[1]),
                            ("压机定位指令报警", status[2]),
                            ("安全光幕报警", status[3]),
                            ("光栅尺计数错误", status[4]),
                            ("超设备压力量程", status[5]),
                            ("手动压力保护", status[6]),
                            ("待机位大于第一位置", status[7]),

                            ("第一位置大于第二位置", status[24]),
                            ("第二位置大于第三位置", status[25]),
                            ("压力判断起点大于终点", status[26]),
                            ("压力判断最小压力大于最大压力", status[27]),
                            ("速度位置参数不能小于0", status[28]),
                            ("速度不能为0", status[29]),
                            ("X轴定位指令报警", status[30]),
                            ("PC通讯异常", status[31]),


                            ("未检测到轴承报警", status[16]),
                            ("未扫码报警", status[17]),
                            ("光幕报警", status[18]),
                            ("压机暂停",status1)
                        };
                            var sendData = alarmList.Where(x => x.Item2 == true).Select(x => x.Item1).ToList();
                            AlarmToUIModels.CreateNormalAlarm(sendData).Send();
                        }
                    }
                    #endregion


                    Thread.Sleep(sleepTime);
                }
            }
            catch (Exception e)
            {
                XLogGlobal.Logger?.LogFatal(e.Message);
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接失败！将在五秒都重新尝试连接", 3000); });
            }

            Thread.Sleep(5000);
        }
    }



    public class AlarmMessages
    {
        public string AlarmMessage { get; set; }
        public bool IsSuccess { get; set; }
    }


    private void SerCode()
    {
        SerialPort serialPort = new SerialPort()
        {
            PortName = Config.Common.Com,
            BaudRate = 115200,
            DataBits = 8,
            StopBits = StopBits.One,
            Parity = Parity.None
        };
        serialPort.Open();

        byte[] buffer = new byte[2048];
        DateTime startTime = DateTime.Now;
        int existCount = 0;

        #region 扫码

        while (true)
        {
            int count = serialPort.BytesToRead;

            if (count > 0)
            {
                serialPort.Read(buffer, existCount, count);
                existCount += count;
                startTime = DateTime.Now;
            }

            if (existCount > 0 && (DateTime.Now - startTime).TotalMilliseconds > 200)
            {
                var canCode = PlotStatus.GetAllEnd();
                if (canCode)
                {
                    if (Common.UsingCodeList)
                    {
                        var temp = Encoding.ASCII.GetString(buffer, 0, existCount);
                        if (this.Code == _codeTemplate)
                        {
                            Code = temp.Trim('\r').Replace("\r", "").Replace("\n", "");
                        }
                        else
                        {
                            _CodeList.Enqueue(temp.Trim('\r').Replace("\r", "").Replace("\n", ""));
                        }
                    }
                    else
                    {
                        Code = Encoding.ASCII.GetString(buffer, 0, existCount);

                        Code = Code.Trim('\r').Replace("\r", "").Replace("\n", "");
                        existCount = 0;
                        if (PlcConnect.GoOn)
                        {
                            PlcConnect.Plc.Write("DB4.18.0", true);
                        }
                    }
                }
                else if (Common.UsingCodeList)
                {
                    var temp = Encoding.ASCII.GetString(buffer, 0, existCount);

                    _CodeList.Enqueue(temp.Trim('\r').Replace("\r", "").Replace("\n", ""));
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("曲线记录中 无法执行扫码"); });
                }
            }

            Thread.Sleep(40);
        }

        #endregion
    }

    private void Startup()
    {
        //Task.Factory.StartNew(Comm, TaskCreationOptions.LongRunning);
        //Task.Factory.StartNew(SerCode, TaskCreationOptions.LongRunning);
        WeakReferenceMessenger.Default.Register<PressMacchineResultWeak>(this, PressResult);
        Siemens3Startup();
    }


    private void PressResult(object recipient, PressMacchineResultWeak message)
    {
        DispatcherHelper.CheckBeginInvokeOnUI(() =>
        {
            if (message.PressMachineResultWeakEnum is PressMachineResultWeakEnum.None)
            {
                switch (message.Token)
                {
                    case
                        "1":
                        PlotStatus.Plot01End = true;
                        PloModelControlDt10.Result = message.Result;
                        break;
                    case
                        "2":
                        PlotStatus.Plot02End = true;
                        PloModelControlDt20.Result = message.Result;
                        break;
                    case
                        "3":
                        PlotStatus.Plot03End = true;
                        PloModelControlDt30.Result = message.Result;
                        break;
                    case
                        "4":
                        PlotStatus.Plot04End = true;
                        PloModelControlDt40.Result = message.Result;
                        break;
                    case
                        "5":
                        PlotStatus.Plot05End = true;
                        PloModelControlDt50.Result = message.Result;
                        break;
                }
            }
            else if (message.PressMachineResultWeakEnum is PressMachineResultWeakEnum.Clear)
            {
                switch (message.Token)
                {
                    case
                        "1":

                        PloModelControlDt10.Result = "";
                        break;
                    case
                        "2":

                        PloModelControlDt20.Result = "";
                        break;
                    case
                        "3":

                        PloModelControlDt30.Result = "";
                        break;
                    case
                        "4":

                        PloModelControlDt40.Result = "";
                        break;
                    case
                        "5":

                        PloModelControlDt50.Result = "";
                        break;
                }
            }

        });
    }
}