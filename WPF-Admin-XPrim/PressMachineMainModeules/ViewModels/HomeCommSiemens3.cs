using HandyControl.Controls;
using HslCommunication;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using System.Diagnostics;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Helper;
using Edge = PressMachineMainModeules.Models.Edge;
using Signal = PressMachineMainModeules.Models.Signal;

namespace PressMachineMainModeules.ViewModels
{
    public partial class HomeViewModel
    {
        private void Siemens3Startup()
        {
            Task.Factory.StartNew(CommSiemens01, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(CommSiemens02, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(CommSiemens03, TaskCreationOptions.LongRunning);
        }



        private void CommSiemens01()
        {
            int sleepTime = 500;
            var relink = 1;
            while (true)
            {

                var linkResult = LinkSiemensPlcConnect(relink);
                if (!linkResult)
                {
                    relink++;
                }
                if (relink == 10)
                {
                    relink = 1;
                }
                try
                {
                    if (linkResult)
                    {
                        relink = 1;
                    }

                    while (PlcConnect.GoOnS7_01 && PlcConnect.GoOnS7_02 && PlcConnect.GoOnS7_03)
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

                        // 扫码完成信号
                        var codestatus = PlcConnect.PlcS701?.ReadBool("DB6.0.0");




                        Thread.Sleep(sleepTime);
                    }
                }
                catch (Exception e)
                {
                    PlcConnect.GoOnS7_03 = false;
                    PlcConnect.GoOnS7_02 = false;
                    PlcConnect.GoOnS7_01 = false;
                    XLogGlobal.Logger?.LogFatal(e.Message);
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接失败！将在五秒都重新尝试连接", 3000); });
                }
                var pingStatus = true;

                while (pingStatus)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("正在PING PLC1", 3000); });
                    var ret = PlcConnect.PingPlcIp(1);
                    if (!ret) { Thread.Sleep(3000); continue; }
                   
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("正在PING PLC2", 3000); });
                    ret = PlcConnect.PingPlcIp(2);
                    if (!ret) { Thread.Sleep(3000); continue; }
                    
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("正在PING PLC3", 3000); });
                    ret = PlcConnect.PingPlcIp(3);
                    if (!ret) { Thread.Sleep(3000); continue; }

                    pingStatus = false;
                }
            }
        }

        private void CommSiemens02()
        {
            Signal signalPress1 = new Signal();
            Signal signalPress2 = new Signal();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int sleepTime = 50;
            var beat = 1;
            while (true)
            {

                try
                {
                    while (PlcConnect.GoOnS7_02)
                    {
                        if (stopwatch.ElapsedMilliseconds >= 300)
                        {
                            beat = beat == 1 ? 0 : 1;
                            var heatBeat = PlcConnect.PlcS702.Write("DB6.4", (ushort)beat);
                            if (!heatBeat.IsSuccess)
                            {
                                throw new Exception("PLC交互异常 心跳异常");
                            }

                            stopwatch.Restart();
                        }

                        // 扫码完成信号
                        var codestatus = PlcConnect.PlcS702?.ReadBool("DB6.0.0");

                        // PressMachine 步序号
                        var step1 = PlcConnect.PlcS702?.ReadInt16("DB6.20");
                        var step2 = PlcConnect.PlcS702?.ReadInt16("DB6.50");
                        if (step1 is null || step2 is null)
                        {
                            throw new Exception("PlcS7_02交互异常 步序号异常- DB6.20 DB6.50");
                        }
                        else if (!step1.IsSuccess)
                        {
                            throw new Exception($"PlcS7_02交互异常 步序号异常- DB6.20  {step1.Message}");
                        }
                        else if (!step2.IsSuccess)
                        {
                            throw new Exception($"PlcS7_02交互异常 步序号异常- DB6.50 {step2.Message}");
                        }

                        // 启停 [Press1,Press2]
                        var start = PlcConnect.PlcS702?.ReadBool("DB6.0.1", 2);
                        if (start is not null && start.IsSuccess)
                        {
                            StartupDrawingLine(signalPress1, signalPress2, start, step1.Content, step2.Content);
                        }
                        else
                        {
                            throw new Exception("PLC交互异常 启停信号异常-DB6.0.1");
                        }

                        // Press1 位置压力 Press2 位置压力
                        var positionPress1 = PlcConnect.PlcS702?.ReadFloat("DB6.12", 2);
                        var positionPress2 = PlcConnect.PlcS702?.ReadFloat("DB6.42", 2);

                        if (positionPress1 is null || positionPress2 is null)
                        {
                            throw new Exception("PlcS7_02交互异常 位置压力异常- DB6.12 DB6.42");
                        }
                        else if (!positionPress1.IsSuccess)
                        {
                            throw new Exception($"PlcS7_02交互异常 位置压力异常- DB6.12  {positionPress1.Message}");
                        }
                        else if (!positionPress2.IsSuccess)
                        {
                            throw new Exception($"PlcS7_02交互异常 位置压力异常- DB6.42 {positionPress2.Message}");
                        }
                        else
                        {
                            PressMahineDataToView(positionPress1.Content, positionPress2.Content);
                        }


                        Thread.Sleep(sleepTime);
                    }
                }
                catch (Exception e)
                {
                    PlcConnect.GoOnS7_02 = false;
                    XLogGlobal.Logger?.LogFatal(e.Message);
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接失败！将在五秒都重新尝试连接", 3000); });
                }

                Thread.Sleep(5000);
            }
        }
        private void CommSiemens03()
        {
            Signal signal = new Signal();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int sleepTime = 50;
            var beat = 1;
            while (true)
            {

                try
                {
                    while (PlcConnect.GoOnS7_03)
                    {
                        if (stopwatch.ElapsedMilliseconds >= 300)
                        {
                            beat = beat == 1 ? 0 : 1;
                            var heatBeat = PlcConnect.PlcS703?.Write("DB6.4", (ushort)beat);
                            if (heatBeat is null || !heatBeat.IsSuccess)
                            {
                                throw new Exception("PLC交互异常 心跳异常");
                            }

                            stopwatch.Restart();
                        }
                        // 位置 压力
                        var ret01 = PlcConnect.PlcS703?.ReadFloat("DB6.12", 2);
                        if (ret01 is null || !ret01.IsSuccess)
                        {
                            throw new Exception("PlcS7_03交互异常 位置压力异常- DB6.12");
                        }
                        else
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                PloModelControlDt50.NowPos = (float)Math.Round(ret01.Content[0], 2);
                                PloModelControlDt50.NowPre = (float)Math.Round(ret01.Content[1], 2);
                            });
                        }

                        // 启停
                        var start = PlcConnect.PlcS703?.ReadBool("DB6.0.1");
                        if (start is null || !start.IsSuccess)
                        {
                            throw new Exception("PlcS7_03交互异常 启停异常- DB6.0.1");
                        }
                        else
                        {
                            signal.Update(start.Content);
                            if (signal.State == Edge.Rising)
                            {
                                if (PlotViewModel05.IsUsing)
                                {
                                    Task.Run(() => { PlotViewModel05.Test(Code); });
                                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"Press3压机启动"); });
                                }
                                else
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("Press3无法重复启动"); });
                                }
                            }
                        }

                        Thread.Sleep(sleepTime);
                    }
                }
                catch (Exception e)
                {
                    PlcConnect.GoOnS7_03 = false;
                    XLogGlobal.Logger?.LogFatal(e.Message);
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("PLC连接失败！将在五秒都重新尝试连接", 3000); });
                }

                Thread.Sleep(5000);
            }
        }

        private void StartupDrawingLine(Signal signalPress1, Signal signalPress2, OperateResult<bool[]>? start,
            short step1, short step2
            )
        {
            if (start == null || start.Content == null)
            {
                return;
            }
            signalPress1.Update(start.Content[0]);
            signalPress2.Update(start.Content[1]);

            if (signalPress1.State == Edge.Rising)
            {
                if (step1 == 1)
                {
                    if (PlotViewModel01.IsUsing)
                    {
                        Task.Run(() => { PlotViewModel01.Test(Code); });
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"Press1压机启动{step1}"); });
                    }
                    else
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("Press1无法重复启动"); });
                    }
                }
                else
                {
                    if (PlotViewModel03.IsUsing)
                    {
                        Task.Run(() => { PlotViewModel03.Test(Code); });
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"Press1压机启动{step1}"); });
                    }
                    else
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("Press1无法重复启动"); });
                    }
                }


            }
            if (signalPress2.State == Edge.Rising)
            {
                if (step2 == 1)
                {
                    if (PlotViewModel02.IsUsing)
                    {
                        Task.Run(() => { PlotViewModel02.Test(Code); });
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"Press2压机启动{step2}"); });
                    }
                    else
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("Press2无法重复启动"); });
                    }
                }
                else
                {
                    if (PlotViewModel04.IsUsing)
                    {
                        Task.Run(() => { PlotViewModel04.Test(Code); });
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"Press2压机启动{step2}"); });
                    }
                    else
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show("Press2无法重复启动"); });
                    }
                }

            }
        }

        private void PressMahineDataToView(float[] press1, float[] press2)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                PloModelControlDt10.NowPos = (float)Math.Round(press1[0], 2);
                PloModelControlDt10.NowPre = (float)Math.Round(press1[1], 2);


                PloModelControlDt20.NowPos = (float)Math.Round(press2[0], 2);
                PloModelControlDt20.NowPre = (float)Math.Round(press2[1], 2);

                PloModelControlDt30.NowPos = (float)Math.Round(press1[0], 2);
                PloModelControlDt30.NowPre = (float)Math.Round(press1[1], 2);

                PloModelControlDt40.NowPos = (float)Math.Round(press2[0], 2);
                PloModelControlDt40.NowPre = (float)Math.Round(press2[1], 2);
            });
        }

        private bool LinkSiemensPlcConnect(int linkSum)
        {
            var result = new List<bool>();
            Enumerable.Range(1, 3).ToList().ForEach(index =>
            {
                var tResult = PlcConnect.LineFromPCToPLC_S7(index).PLC_S7ConnectErrorView(index, linkSum);
                result.Add(tResult);
                if (!tResult)
                    return;
            });

            return result.Count(item => item) == 3;
        }
    }

    public static class BoolSupplemented
    {
        public static bool PLC_S7ConnectErrorView(this bool connectResult, int connectId, int linkSum)
        {
            if (connectResult)
            {
                XLogGlobal.Logger?.LogInfo($"PlcS7_0{connectId}连接成功");
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"PlcS7_0{connectId}连接成功"); });
            }
            else
            {
                XLogGlobal.Logger?.LogInfo($"PlcS7_0{connectId}连接失败！将在{linkSum * 5000}毫秒后都重新尝试连接");
                Growl.WarningGlobal($"PlcS7_0{connectId}连接失败！将在{linkSum * 5000}毫秒后重新尝试连接");
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"PlcS7_0{connectId}连接失败！将在{linkSum * 5000}毫秒后重新尝试连接", 3000); });
                Thread.Sleep(1000);
            }
            return connectResult;
        }
    }
}
