using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot.Series;
using OxyPlot;
using WPF.Admin.Models;
using System.Collections.ObjectModel;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot.Annotations;
using System.Text.RegularExpressions;
using WPF.Admin.Service.Services;
using PressMachineMainModeules.Config;
using System.Text.Json;
using HandyControl.Controls;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Themes.Converter;
using Edge = PressMachineMainModeules.Models.Edge;
using Signal = PressMachineMainModeules.Models.Signal;
using HslCommunication.Profinet.Siemens;
using HslCommunication.Core.Device;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PlotViewModel : BindableBase
    {
        /// <summary>
        /// 防止曲线没画完 又开始画曲线
        /// </summary>
        public bool IsUsing { get; set; } = true;

        /// <summary>
        /// 开始结束
        /// </summary>
        public string? Drawing { get; init; }

        /// <summary>
        /// 位移
        /// </summary>
        public string? PressPos { get; init; }

        /// <summary>
        /// 压力
        /// </summary>
        public string? PressPre { get; init; }

        /// <summary>
        /// 结果  
        /// </summary>
        public string? PressRes { get; init; }

        /// <summary>
        /// 电钢号
        /// </summary>
        public string? PressMachineNo { get; init; }


        #region UI

        [ObservableProperty] private string? pressMachineName;

        LineSeries _rawLineSeries = new()
        {
            TrackerFormatString = "{1}: {2:000.000}\n{3}: {4:000.000}",
            Color = OxyColors.Red,
        };

        #endregion

        #region

        private bool _monitorRecAlarm = false;
        private List<Measurement> _baseData = new();
        public Signal RecordSignal = new Signal();

        public ObservableCollection<MonitorRec> MonitorRecs { get; set; }
            = new ObservableCollection<MonitorRec>();

        public string? RecipeName { get; set; } = string.Empty;

        #endregion

        #region

        [ObservableProperty] private ObservableCollection<Measurement> _rawData = new();
        [ObservableProperty] private PlotModel? _plotModel;
        [ObservableProperty] private CurvePara? _curveParaN = Config.Common.CurvePara;
        [ObservableProperty] private float _maxPre;
        [ObservableProperty] private float _endPre;

        #endregion

        private readonly object _syncLock = new object();
        private DateTime _lastUpdateTime = DateTime.Now;
        private const int UPDATE_INTERVAL_MS = 50; // 50ms 刷新一次

        public PlotViewModel(string drwaing, string pressPos,
            string pressPre, string pressRes, string pressMachineNo, string? pressMachineName, DeviceCommunication? plc = null)
        {
            this.PressPre = pressPre;
            this.PressRes = pressRes;
            this.Drawing = drwaing;
            this.PressPos = pressPos;
            this.PressMachineNo = pressMachineNo;
            PressMachineName = pressMachineName;
            if (plc != null)
            {
                Plc = plc;
            }
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                PlotModel = PlotHelper.CreatePlotModel(-20, 20, 0, 100);

                // 配置曲线
                _rawLineSeries = new LineSeries
                {
                    Title = "压力曲线",
                    TrackerFormatString = "位移: {2:0.000}mm\n压力: {4:0.000}N",
                    Color = OxyColors.Red,
                    StrokeThickness = 1.5,
                    //CanTrackerInterpolatePoints = true,
                    InterpolationAlgorithm = new CatmullRomSpline(0.1)
                };

                PlotModel.Series.Add(_rawLineSeries);

                // 设置 PlotModel 的交互属性
                PlotModel.IsLegendVisible = true;
            });
            //InitTestData();
        }

        private void InitTestData()
        {
            var random = new Random();

            // 清除现有点
            _rawLineSeries.Points.Clear();

            // 添加测试数据点
            for (int i = 0; i < 50; i++)
            {
                double x = i * 0.5 - 10; // 从-10开始
                double y = 20 + 30 * Math.Sin(x * 0.5) + random.NextDouble() * 5;

                DispatcherHelper.CheckBeginInvokeOnUI(() => { _rawLineSeries.Points.Add(new DataPoint(x, y)); });
            }

            // 刷新图表
            DispatcherHelper.CheckBeginInvokeOnUI(() => { PlotModel!.InvalidatePlot(true); });
        }

        // 如果需要实时更新数据
        public void AddNewPoint(double x, double y)
        {
            lock (_syncLock)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { });

                _rawLineSeries.Points.Add(new DataPoint(x, y));

                // 如果点数太多，移除旧的点
                if (_rawLineSeries.Points.Count > 100)
                {
                    _rawLineSeries.Points.RemoveAt(0);
                }

                // 控制刷新频率
                var now = DateTime.Now;
                if ((now - _lastUpdateTime).TotalMilliseconds >= UPDATE_INTERVAL_MS)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        PlotModel?.InvalidatePlot(false); // 使用 false 减少重绘开销
                    });
                    _lastUpdateTime = now;
                }
            }
        }

        #region 更换判断框

        public void ChangeParamOxyPlotView(CurvePara CurvePara,
            ObservableCollection<MonitorRec> monitorRecs, string name)
        {
            RecipeName = name;
            MonitorRecs = monitorRecs;
            CurveParaN = CurvePara;
            PlotModel!.Annotations.Clear();
            PlotHelper.CreateMonitorRec(PlotModel!, new List<MonitorRec>(monitorRecs));
            var xAxis = PlotModel!.Axes.FirstOrDefault(a => a.Key == "HorizontalAxis");
            var yAxis = PlotModel.Axes.FirstOrDefault(a => a.Key == "VerticalAxis");
            if (xAxis != null && yAxis != null)
            {
                xAxis.Minimum = CurvePara.MinX;
                xAxis.Maximum = CurvePara.MaxX;
                yAxis.Minimum = CurvePara.MinY;
                yAxis.Maximum = CurvePara.MaxY;
            }

            PlotModel.ResetAllAxes();
            PlotModel.InvalidatePlot(false);
        }
        [ObservableProperty] private double _ShowCount = 10;
        private void SetPlotViewSize(double maxX)
        {
            var xAxis = PlotModel!.Axes.FirstOrDefault(a => a.Key == "HorizontalAxis");
            var yAxis = PlotModel.Axes.FirstOrDefault(a => a.Key == "VerticalAxis");
            //var count = xAxis?.Maximum - xAxis?.Minimum;
            if (xAxis != null && yAxis != null)
            {
                xAxis.Minimum = maxX - ShowCount;
                xAxis.Maximum = maxX;

            }


            PlotModel.ResetAllAxes();
        }

        #endregion

        #region

        private List<Measurement> CreateFittedMeas(double[] fittedPositions, double[] fittedPressures)
        {
            List<Measurement> fittedMeas = new List<Measurement>();
            for (int i = 0; i < fittedPositions.Length; i++)
            {
                fittedMeas.Add(new Measurement
                {
                    Position = (float)fittedPositions[i],
                    Pressure = (float)fittedPressures[i]
                });
            }

            return fittedMeas;
        }

        private Result CalcuTotalRet(IList<MonitorRec> recs)
        {
            var totalRet = recs.All(rec => rec.Result == Result.Ok);
            return totalRet ? Result.Ok : Result.Ng;
        }

        #endregion

        #region 编码

        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 允许所有字符
            WriteIndented = true, // 格式化输出
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // 驼峰命名
        };

        #endregion

        public int AddPosLength { get; set; } // 插入长度
        public DeviceCommunication Plc { get; set; }
        /// <summary>
        /// 自动画曲线
        /// </summary>
        public async void Test(string codeBase = "未扫码", DeviceCommunication? plc = null)
        {
            WeakReferenceMessenger.Default.Send(PressMacchineResultWeak.CreateClearWeak(PressMachineNo));

            //PlotModel?.Series.Add(_rawLineSeries);
            if (codeBase is "等待扫码中")
            {
                codeBase = "未扫码";
            }
            if (plc != null)
            {
                Plc = plc;
            }

            if (Plc is null)
            {
                throw new ArgumentNullException(nameof(Plc), "PLC连接对象不能为空");
            }

            IsUsing = false;
            var index = true;
            var startTime = DateTime.Now;
            DateTime lastUpdatePlotTime = DateTime.Now;
            var recording = false;
            PointAnnotation inflectionAnno = null;
            bool dataChanged = false;
            float endPostions = -1;
            double nowSize = 100;
            while (PlcConnect.GoOn)
            {
                try
                {
                    // UI、刷新
                    if (dataChanged)
                    {
                        var span = (DateTime.Now - lastUpdatePlotTime).TotalMilliseconds;
                        if (span > 700)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => { PlotModel!.InvalidatePlot(true); });
                            dataChanged = false;
                            lastUpdatePlotTime = DateTime.Now;
                            //SetPlotViewSize(nowSize);
                        }
                    }

                    var isCouinter = Plc?.ReadBool(Drawing);

                    if (isCouinter is not null && isCouinter.IsSuccess)
                    {
                        var isCountion = isCouinter!.Content;
                        RecordSignal.Update(isCountion);
                    }

                    // 记录完成
                    if (RecordSignal.State == Edge.Falling)
                    {
                        recording = false;
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { PlotModel!.InvalidatePlot(true); });
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { PlotModel!.InvalidatePlot(false); });
                        Thread.Sleep(50);
                        if (_baseData.Count > 2)
                        {
                            List<bool> rets = new List<bool>();
                            for (int i = 0; i < MonitorRecs.Count; i++)
                            {
                                var rec = MonitorRecs[i];
                                if (rec.Result == Result.None || rec.MonitorType == MonitorType.BottomInNoOut)
                                {
                                    MonitorRec.Judge(_baseData.ToList(), rec);
                                    //var pre = Inovance.InovanceTcpNet?.ReadFloat(dto.最大压力);
                                    //if (rec.MonitorType == MonitorType.BottomInNoOut)
                                    //{
                                    //    if (pre.Content < rec.MinY || pre.Content > rec.MaxY) 
                                    //        rec.Result = Result.Ng;
                                    //}
                                    if (rec.Result == Result.Ng)
                                    {
                                        var alarmRet = Plc?.Write(PressRes, (ushort)2);

                                        WeakReferenceMessenger.Default.Send(new PressMacchineResultWeak
                                        {
                                            InflectionPos = 0,
                                            InflectionPre = 0,
                                            Token = PressMachineNo,
                                            Result = "NG",
                                        });

                                        _monitorRecAlarm = true;
                                    }
                                }
                            }

                            _monitorRecAlarm = rets.Contains(false);

                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                MaxPre = _baseData.Max(d => d.Pressure);
                                EndPre = _baseData[_baseData.Count - 1].Position;
                            });

                            if (DataGenerator.FitDataAndUpdate(
                                    _baseData,
                                    out double[] fittedTimes,
                                    out double[] fittedPositions,
                                    out double[] fittedPressures))
                            {
                                //_baseData = new List<Model.OxyPlot.Measurement>();
                                //for (int i = 0; i < fittedPositions.Count(); i++)
                                //{
                                //    _baseData.Add(new Model.OxyPlot.Measurement()
                                //    {
                                //        Position=float.Parse( fittedPositions[i].ToString()),
                                //        Pressure= float.Parse(fittedPressures[i].ToString()),
                                //    });
                                //}

                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    _rawLineSeries.IsVisible = true;
                                    //_fittedLineSeries.IsVisible = true;
                                    PlotModel!.InvalidatePlot(false);
                                });

                                var fittedMeas = CreateFittedMeas(fittedPositions, fittedPressures);

                                Result totalRet = Result.Ok;

                                if (MonitorRecs.Count > 0)
                                {
                                    totalRet = CalcuTotalRet(MonitorRecs);
                                }

                                double inflectionPos = -9999;
                                double inflectionPres = -9999;

                                // 绘制拐点
                                if (Config.Common.DetectInflection)
                                {
                                    var found = InflectionHelper.FindInflection(fittedPositions, fittedPressures,
                                        out inflectionPos, out inflectionPres,
                                        CurveParaN!);
                                    Thread.Sleep(500);
                                    if (found)
                                    {
                                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                        {
                                            inflectionAnno = PlotHelper.CreateInflection(PlotModel!,
                                                (float)inflectionPos, (float)inflectionPres, true);
                                        });
                                    }

                                    if (!found)
                                    {
                                        totalRet = Result.Ng;
                                    }
                                    else
                                    {
                                        if (Config.Common.InflectionPosLimit)
                                        {
                                            if (inflectionPres < CurveParaN!.InflexionMinY
                                                || inflectionPres > CurveParaN.InflexionMaxY
                                                || inflectionPos < CurveParaN.InflexionMinX
                                                || inflectionPos > CurveParaN.InflexionMaxX)
                                            {
                                                totalRet = Result.Ng;
                                            }
                                        }
                                        else
                                        {
                                            if (inflectionPres < CurveParaN!.InflexionMinY ||
                                                inflectionPres > CurveParaN.InflexionMaxY)
                                            {
                                                totalRet = Result.Ng;
                                            }
                                        }
                                    }
                                }

                                WeakReferenceMessenger.Default.Send(new PressMacchineResultWeak
                                {
                                    InflectionPos = (float)inflectionPos,
                                    InflectionPre = (float)inflectionPres,
                                    Token = PressMachineNo,
                                    Result = totalRet == Result.Ok ? "OK" : "NG",
                                });

                                if (totalRet == Result.Ok)
                                {
                                    Plc?.Write(PressRes, (ushort)1);
                                }
                                else
                                {
                                    Plc?.Write(PressRes, (ushort)2);
                                }

                                string recipeFolder = $"{Config.Common.DataFolder}\\" +
                                                      DateTime.Now.ToString("yyyy年MM月dd日");
                                CreateDirPath.CreateFolderIfNotExist(recipeFolder);
                                var code = codeBase;
                                string DealCode = Regex.Replace(code!, "[^a-zA-Z0-9\u4e00-\u9fa5% ._]", string.Empty);

                                string fold = totalRet == Result.Ok
                                    ? recipeFolder + $"\\{DealCode}\\OK"
                                    : recipeFolder + $"\\{DealCode}\\NOK";
                                CreateDirPath.CreateFolderIfNotExist(fold);
                                // +"\\"+ $"{RecipeName}\\"

                                string filename = fold + "\\" + RecipeName +
                                                  $"_{DealCode}_{PressMachineNo}_{DateTime.Now.ToString("HH时mm分ss秒")}.xlsx";


                                var curveParaTemp = new CurvePara
                                {
                                    MinX = CurveParaN!.MinX,
                                    MaxX = CurveParaN.MaxX,
                                    MinY = CurveParaN.MinY,
                                    MaxY = CurveParaN.MaxY,
                                    InflexionMinX = CurveParaN.InflexionMinX,
                                    InflexionMaxX = CurveParaN.InflexionMaxX,
                                    InflexionMinY = CurveParaN.InflexionMinY,
                                    InflexionMaxY = CurveParaN.InflexionMaxY
                                };
                                string SaveModel = ""; //模式
                                DataExcelHelper.Save(
                                    filename,
                                    RecipeName!,
                                    DealCode!,
                                    totalRet,
                                    (float)inflectionPos,
                                    (float)inflectionPres,
                                    Config.Common.Steps,
                                    MonitorRecs,
                                    curveParaTemp,
                                    null, // 不保存原始数据
                                    null,
                                    fittedTimes,
                                    fittedPositions,
                                    fittedPressures,
                                    SaveModel,
                                    null, EndPre, MaxPre,
                                    "6151"); //EXCEL密码                               
                                var savemodel = new PreSaveModel
                                {
                                    Code = DealCode,
                                    Operator = LoginAuthHelper.LoginUser.UserName,
                                    CreateTime = DateTime.Now,
                                    Reuslt = totalRet == Result.Ok ? "OK" : "NG",
                                    Data = $"配方:{RecipeName},电缸号：{PressMachineNo}",
                                    PoltFilePath = filename
                                };

                                Task.Run(async () =>
                                {
                                    var saveSQLRes = await PressMachineDataHelper.Save(savemodel);
                                    if (!saveSQLRes)
                                    {
                                        var saveString = JsonSerializer.Serialize(savemodel, options);
                                        XLogGlobal.Logger?.LogError("数据库保存失败数据源：\n" + saveString);
                                        DispatcherHelper.CheckBeginInvokeOnUI(() => { Growl.ErrorGlobal("数据保存失败"); });
                                    }
                                });

                                // 清除结果状态                              
                                foreach (var rec in MonitorRecs)
                                {
                                    rec.Result = Result.None;
                                }
                            }

                            ;
                        }

                        IsUsing = true;
                        return;
                    }

                    // 开始记录
                    if (index)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            for (int i = 0; i < MonitorRecs.Count; i++)
                            {
                                MonitorRecs[i].Result = Result.None;
                            }
                            _rawLineSeries.IsVisible = true;
                            RawData.Clear();
                            //PlotModel.Series.Add(_rawLineSeries);

                            _rawLineSeries.Points.Clear();
                            PlotHelper.RemoveInflection(PlotModel!, inflectionAnno!);
                            PlotModel!.InvalidatePlot(true);
                        });
                        AddPosLength = 0;
                        _baseData.Clear();
                        index = false;
                        recording = true;
                    }
                    //var stop = PlcConnect.Plc.ReadBool("M2000.5");
                    // 记录中
                    if (recording)
                    {
                        var pos = Plc?.ReadFloat(PressPos);
                        var pre = Plc?.ReadFloat(PressPre);
                        if (pos is not null && pos!.IsSuccess)
                        {
                            if (endPostions <= pos.Content || true)
                            {
                                endPostions = (float)pos.Content;
                                if ((float)pre!.Content < 10000)
                                {
                                    Measurement measurement = new Measurement
                                    {
                                        TimeStamp = DateTime.Now,
                                        //Position = (float)pos.Content,
                                        Position = (float)pos.Content,
                                        Pressure = (float)pre!.Content,
                                    };
                                    DateTime timeStamp = measurement.TimeStamp;
                                    float position = measurement.Position;
                                    float pressure = measurement.Pressure;
                                    _baseData.Add(measurement.GetCopy());
                                    nowSize = position;
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        _rawLineSeries.Points.Add(new DataPoint(measurement.Position, measurement.Pressure));
                                        RawData.Add(measurement);
                                    });
                                    //if (_baseData.Count > 100000)
                                    //{
                                    //    _baseData.RemoveAt(0);
                                    //    _rawLineSeries.Points.RemoveAt(0);
                                    //    RawData.RemoveAt(0);
                                    //}


                                    foreach (var rec in MonitorRecs)
                                    {
                                        if (rec.MonitorType != MonitorType.BottomInNoOut
                                            && rec.MonitorType != MonitorType.LeftInRightNoOut
                                            && position > rec.MaxX && rec.Result == Result.None)
                                        {
                                            MonitorRec.Judge(_baseData, rec);

                                            if (rec.Result == Result.Ng)
                                            {
                                                WeakReferenceMessenger.Default.Send(new PressMacchineResultWeak
                                                {
                                                    InflectionPos = 0,
                                                    InflectionPre = 0,
                                                    Token = PressMachineNo,
                                                    Result = "NG",
                                                });
                                                var alarmRet = PlcConnect.Plc?.Write(PressRes, (ushort)2);
                                                if (alarmRet!.IsSuccess)
                                                {
                                                    //UpdateRets(Result.Ng, (float)0.00, (float)0.00);
                                                    //throw new Exception("PLC连接中断！");
                                                }

                                                _monitorRecAlarm = true;
                                            }
                                        }
                                    }

                                    dataChanged = true;
                                }
                                else
                                {

                                }






                            }
                        }


                    }

                    //Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    XLogGlobal.Logger?.LogError(ex.Message);
                    //WeakReferenceMessenger.Default.Send<Exception>(ex);
                    IsUsing = true;
                    return;
                }
            }
        }
    }
}