using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;
using Edge = PressMachineMainModeules.Models.Edge;

namespace PressMachineMainModeules.ViewModels
{
    public partial class AutoHomeViewModel : BindableBase
    {
        [ObservableProperty] private ObservableCollection<PlotUtils> _plotModels;
        [ObservableProperty] private string _code = string.Empty;

        [ObservableProperty] private AutoCodeModel? _autoPartialCodeModel;
        private AutoMesProperties AutoMesProperties { get; set; } = new AutoMesProperties();

        private AutoSignalInteractionSetupViewModel autoSignalInteractionSetupViewModel;

        public AutoHomeViewModel()
        {
            AutoPartialCodeModel = new AutoCodeModel();
            autoSignalInteractionSetupViewModel = new AutoSignalInteractionSetupViewModel();
            var tempPlotUtils = new List<PlotUtils>();
            foreach (var item in HomeManager.Instance.HomePositionModels)
            {
                var plot = new AutoPlotViewModel(
                    item.StartPosition, item.PositionPosition.Split('-')[0], item.PressPosition.Split('-')[0],
                    item.ResultPosition, item.KeyNum, item.Desc, ConfigPlcs.Instance[item.PlcName]
                )
                {
                    StartStep = item.StartStep,
                    FormulaNum = item.FormulaNum,
                };
                var findAutoCode = AutoPartialCodeModel.PartialCodes.FirstOrDefault(e => e.AutoMode == item.Desc);
                plot.SetAutoCode(findAutoCode);

                plot.AutoPlotValues.AddHomePosition(item);

                var plotUtil = new PlotUtils
                {
                    Key = int.Parse(item.KeyNum),
                    Name = item.Desc,
                    PlotInfo = item,
                    Plot = plot,
                    Startup = item.StartPosition,
                    PlcName = item.PlcName,
                    StartStep = item.StartStep,
                    Step = item.Step,
                    United = item.United,
                    UnitedValue = item.UnitedValue
                };
                tempPlotUtils.Add(plotUtil);
            }

            tempPlotUtils = tempPlotUtils.OrderBy(e => e.Key).ToList();
            PlotModels = new ObservableCollection<PlotUtils>(tempPlotUtils);

            Task.Run(() => { AlarmManager.Instance.Initialized(ConfigPlcs.ConfigPath, "Alarm"); });

            WeakReferenceMessenger.Default.Register<AutoParameterWeakRef>(this, ChangedParametersOnHomeView);

            Task.Factory.StartNew(ReadHomeShowValue, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(ReadCodeValue, TaskCreationOptions.LongRunning);

            autoSignalInteractionSetupViewModel.Start();
        }

        public async Task SetCode(string tempCode)
        {
            bool isCheckDb = false;
            if (ConfigCode.Instance.BarcodePlagiarismCheck.IsOpen)
            {
                if (ConfigCode.Instance.Rework.IsOpen)
                {
                    var checkCode = await ConfigPlcs.Instance[ConfigCode.Instance.Rework.PlcName]
                        .ReadBoolAsync(ConfigCode.Instance.Rework.DbPosint);
                    if (checkCode is null || !checkCode.IsSuccess)
                    {
                        Growl.ErrorGlobal($"数据读取失败，条码检测{ConfigCode.Instance.Rework.DbPosint}");
                        return;
                    }

                    isCheckDb = checkCode.Content;
                }

                if (!isCheckDb)
                {
                    var r = await PressMachineDataHelper.CheckCode(tempCode);

                    if (r is not null)
                    {
                        Growl.ErrorGlobal($"条码检测{tempCode} 在 {r.CreateTime} 已存在");
                        return;
                    }
                }
            }


            var result = await AutoCodeHelper.CheckCode(tempCode, AutoMesProperties, AutoPartialCodeModel);
            if (result.Item1)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"识别成功 {result.Item2}"); });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { SnackbarHelper.Show($"识别失败 {tempCode}"); });
                XLogGlobal.Logger?.LogError($"识别失败 {tempCode}");
            }
        }

        private async Task ReadCodeValue()
        {
            if (ConfigCode.Instance.CodeMode == CodeModeEnum.PlcCode)
            {
                var tempCode = string.Empty;
                while (true)
                {
                    try
                    {
                        var code = ConfigPlcs.Instance[ConfigCode.Instance.PlcName]
                            .ReadString(ConfigCode.Instance.Parameter, 100);

                        if (code is null || !code.IsSuccess)
                        {
                            throw new Exception(
                                $"Read Error {ConfigCode.Instance.PlcName} -- {ConfigCode.Instance.Parameter}");
                        }

                        var ycode = code.Content.Trim('\r').Replace("\r", "").Replace("\n", "").Trim('\0');
                        if (tempCode == ycode)
                        {
                            Thread.Sleep(500);
                            continue;
                        }

                        tempCode = ycode;

                        if (!string.IsNullOrEmpty(Code))
                        {
                            await SetCode(tempCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        XLogGlobal.Logger?.LogError(ex.Message, ex);
                        Growl.ErrorGlobal(ex.Message);
                        Thread.Sleep(3000);
                    }

                    Thread.Sleep(500);
                }
            }
            else if (ConfigCode.Instance.CodeMode == CodeModeEnum.ComCode)
            {
                var parameters = ConfigCode.Instance.Parameter.Split('-');
                var parity = parameters[2] switch
                {
                    "None" => Parity.None,
                    "Odd" => Parity.Odd,
                    "Even" => Parity.Even,
                    "Mark" => Parity.Mark,
                    "Space" => Parity.Space,
                    _ => throw new ArgumentException("Invalid parity value")
                };
                SerialPort serialPort = new SerialPort()
                {
                    PortName = parameters[0],
                    BaudRate = int.Parse(parameters[1]),
                    Parity = parity,
                    DataBits = int.Parse(parameters[3]),
                    StopBits = (StopBits)Enum.Parse(typeof(StopBits), parameters[4]),
                };
                serialPort.Open();

                byte[] buffer = new byte[2048];
                DateTime startTime = DateTime.Now;
                int existCount = 0;

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
                        var tempCode = Encoding.ASCII.GetString(buffer, 0, existCount);
                        tempCode = tempCode.Trim('\r').Replace("\r", "").Replace("\n", "");
                        existCount = 0;

                        await SetCode(tempCode);
                    }

                    Thread.Sleep(40);
                }
            }
        }

        private void ReadHomeShowValue()
        {
            //.ConnectServer();

            Stopwatch sp = new Stopwatch();
            sp.Start();
            while (true)
            {
                try
                {
                    var start = false;
                    var result = ConfigPlcs.Instance.ConnectServer();

                    if (result.Any(e =>
                            e.DeviceCommunicationState ==
                            CMS.ReaderConfigLIbrary.Models.DeviceCommunicationState.NotConnect))
                    {
                        start = false;
                        throw new Exception("Connect Message:" + result.FirstOrDefault(e =>
                            e.DeviceCommunicationState ==
                            CMS.ReaderConfigLIbrary.Models.DeviceCommunicationState.NotConnect)?.Message);
                    }
                    else
                    {
                        ConfigPlcs.Instance.StartHeartbeat();
                        start = true;
                    }

                    while (start)
                    {
                        if (PlotModels is not null)
                        {
                            // 报警
                            if (sp.ElapsedMilliseconds > 500)
                            {
                                AlarmReader();
                                sp.Restart();
                            }

                            // 数值
                            foreach (var item in PlotModels)
                            {
                                foreach (var readerItem in item.Plot.AutoPlotValues)
                                {
                                    if (readerItem.Name.Contains("结果"))
                                        continue;
                                    if (readerItem.Name == "合格率")
                                    {
                                        var ok = item.Plot.AutoPlotValues.FirstOrDefault(e => e.Name == "OK数量");
                                        var ng = item.Plot.AutoPlotValues.FirstOrDefault(e => e.Name == "NG数量");
                                        readerItem.Value = (float)
                                            (ok.Value == 0 ? 0 : Math.Round(ok.Value / (ok.Value + ng.Value), 4)) * 100;
                                        continue;
                                    }

                                    if (readerItem.Name.Contains("总数"))
                                    {
                                        var ok = item.Plot.AutoPlotValues.FirstOrDefault(e => e.Name == "OK数量");
                                        var ng = item.Plot.AutoPlotValues.FirstOrDefault(e => e.Name == "NG数量");
                                        readerItem.Value = (int)ok.Value + (int)ng.Value;
                                        continue;
                                    }

                                    readerItem.Read(ConfigPlcs.Instance[readerItem.PlcName], readerItem.DbPoint);
                                }

                                Thread.Sleep(30);
                            }

                            // 启动
                            foreach (var item in PlotModels)
                            {
                                var startbool = ConfigPlcs.Instance[item.PlcName].ReadBool(item.Startup);
                                if (!startbool.IsSuccess)
                                {
                                    throw new Exception($"Read Error {item.PlcName} -- {item.Startup}");
                                }

                                if (item.United) // 联合启动
                                {
                                    var step = item.StepRead(ConfigPlcs.Instance[item.PlcName]);
                                    item.Signal.Update(startbool.Content && step.ToString() == item.UnitedValue);
                                }
                                else
                                {
                                    item.Signal.Update(startbool.Content);
                                }
                                
                                

                                if (item.Signal.State == Edge.Rising)
                                {
                                    if (item.StartStep && !item.United)
                                    {
                                        if (string.IsNullOrEmpty(item.Plot.FormulaNum))
                                        {
                                            var formula = item.FormulaRead(ConfigPlcs.Instance[item.PlcName]);
                                            var step = item.StepRead(ConfigPlcs.Instance[item.PlcName]);
                                            if (formula is null || step is null)
                                            {
                                                throw new Exception(
                                                    $"Read Error {item.PlcName} -- {item.Plot.FormulaNum} -- {item.Step}");
                                            }

                                            AutoInitialized($"{formula}#{step}");
                                        }
                                        else
                                        {
                                            var step = item.StepRead(ConfigPlcs.Instance[item.PlcName]);
                                            if (step is null)
                                            {
                                                throw new Exception($"Read Error {item.PlcName} -- {item.Step}");
                                            }

                                            item.Plot.ChangeStep(((int)step!).ToString());
                                        }
                                    }

                                    if (item.Plot.IsUsing)
                                    {
                                        Task.Run(() => { item.Plot.Test(plc: ConfigPlcs.Instance[item.PlcName]); });
                                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                        {
                                            SnackbarHelper.Show($"{item.Name} startup");
                                        });
                                    }
                                    else
                                    {
                                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                        {
                                            SnackbarHelper.Show($"{item.Name} already startup ! plc edge error");
                                        });
                                    }
                                }
                            }
                        }

                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError(ex.Message, ex);
                    Growl.WarningGlobal(ex.Message);
                    Thread.Sleep(3000);
                }

                Thread.Sleep(100);
            }
        }

        private void ChangedParametersOnHomeView(object recipient, AutoParameterWeakRef message)
        {
            ChangedParametersOnHomeView(message);
        }

        private void ChangedParametersOnHomeView(AutoParameterWeakRef message)
        {
            foreach (var item in message.Parameters)
            {
                var findPlot = PlotModels.FirstOrDefault(e => e.Name == item.Key);

                if (findPlot is null)
                {
                    continue;
                }

                findPlot.Plot.WeakPlotSize(item.Value);
            }
        }

        private void AlarmReader()
        {
            if (AlarmManager.Instance.AlarmPositions.Count == 0)
            {
                return;
            }

            var alarmList = new List<Tuple<string, bool>>();

            foreach (var item in AlarmManager.Instance.AlarmPositions)
            {
                var state = ConfigPlcs.Instance[item.PlcName].ReadBool(item.getFullPosition);
                if (state.IsSuccess)
                {
                    alarmList.Add(new Tuple<string, bool>(item.Desc, state.Content));
                }
                else
                {
                    throw new Exception($"IO读取失败，点位：{item.getFullPosition}，错误信息：{state.Message}");
                }
            }

            var sendData = alarmList.Where(x => x.Item2 == true).Select(x => x.Item1).ToList();
            if (sendData.Count >= 0)
            {
                AlarmToUIModels.CreateNormalAlarm(sendData).Send();
            }
        }


        private void AutoInitialized(string name)
        {
            string filename = System.IO.Path.Combine(AutoParameterViewModel.dir, $"{name}.json");

            if (!System.IO.File.Exists(filename))
            {
                return;
            }

            var jsonData = SerializeHelper.Deserialize<AutoParameterSaveEntity>(filename);
            AutoParameterModel data = DeepCopyHelper.JsonClone(AutoParameterModelManager.Instance.AutoParameterModel);
            if (data is null) return;
            data.GetSaveEntity(jsonData, false);
            var sendModel = data.ToAutoParameterWeakRefEntity(name);
            ChangedParametersOnHomeView(sendModel);
        }


        public void NormalInitialized()
        {
            var configPath =
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "PressConfig.json");
            var pressMachineParam = SerializeHelper.Deserialize<PressMachineHistoryParams>(configPath);
            AutoInitialized(pressMachineParam.ParasName);
        }
    }
}