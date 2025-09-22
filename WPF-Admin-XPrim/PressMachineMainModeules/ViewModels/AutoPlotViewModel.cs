using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using HslCommunication.Core.Device;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Legends;
using OxyPlot.Series;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using StackExchange.Redis;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Media;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Service.Services.Garnets;
using WPF.Admin.Themes.Controls;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;
using static PressMachineMainModeules.Models.AutoParameterWeakRef;
using Edge = PressMachineMainModeules.Models.Edge;
using Signal = PressMachineMainModeules.Models.Signal;

namespace PressMachineMainModeules.ViewModels
{
    public partial class AutoPlotViewModel : BindableBase
    {
        public ObservableCollection<AutoPlotValue> AutoPlotValues { get; set; } =
            new ObservableCollection<AutoPlotValue>();


        private List<AutoParameterWeakTp>? autoParameterWeakTps;


        private AutoPartialCodeContent? AutoPartialCodeContent { get; set; }

        public void SetAutoCode(AutoPartialCodeContent? dto)
        {
            this.AutoPartialCodeContent = dto;
        }

        public string? FormulaNum { get; set; }

        public string GetMainCode(int Step)
        {
            try
            {
                if (AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.SelectStepCheckCodeOpen)
                {
                    return AutoPartialCodeContent?.MainCode[Step] ?? throw new AbandonedMutexException("未扫码");
                }

                return AutoPartialCodeContent?.MainCode[1] ?? throw new AbandonedMutexException("未扫码");
                ;
            }
            catch (Exception e)
            {
                return "未扫码";
            }
        }

        public List<string>? GetPartialCode(int Step)
        {
            if (AutoCheckCodeModelManager.Instance.AutoCheckCodeModel.SelectStepCheckCodeOpen)
            {
                return
                    AutoPartialCodeContent?.PartialCodes[Step].ToList();
            }


            return
                AutoPartialCodeContent?.PartialCodes[1].ToList();
        }


        public void WeakPlotSize(List<AutoParameterWeakTp> dto)
        {
            this.autoParameterWeakTps = dto;

            ChangeStep("1");
        }


        public bool ChangeStep(string step)
        {
            if (this.autoParameterWeakTps is null || this.autoParameterWeakTps.Count < 1)
            {
                return false;
            }

            var findFirstSize = this.autoParameterWeakTps.FirstOrDefault(e => e.AutoParameterWeakDic.Step == step);

            if (!StartStep)
            {
                findFirstSize = this.autoParameterWeakTps[0];
            }

            if (int.TryParse(step, out var value))
            {
                this.AutoMesProperty.Step = value;
            }

            if (findFirstSize is null)
            {
                Growl.ErrorGlobal($"Not Found {autoParameterWeakTps[0].ParameterName} Content {step}");
                return false;
            }

            this.Step = step;

            ChangeParamOxyPlotView(findFirstSize.AutoParameterWeakDic.AutoParameterWeakContent.CurvePara,
                findFirstSize.AutoParameterWeakDic.AutoParameterWeakContent.MonitorRecs, findFirstSize.ParameterName,
                findFirstSize.AutoParameterWeakDic.AutoParameterWeakContent.EnvelopePosintModel);

            Growl.SuccessGlobal($"{autoParameterWeakTps[0].ParameterName}-{step}:切换成功");

            return true;
        }


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

        [ObservableProperty] private EnvelopePosintModel _envelope;

        [ObservableProperty] private string? _RecipeName = string.Empty;

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

        public string Step { get; set; }

        public bool StartStep { get; set; }

        public AutoPlotViewModel(string drwaing, string pressPos,
            string pressPre, string pressRes, string pressMachineNo, string? pressMachineName,
            DeviceCommunication? plc = null)
        {
            this.PressPre = pressPre;
            this.PressRes = pressRes;
            this.Drawing = drwaing;
            this.PressPos = pressPos;

            WeakReferenceMessenger.Default.Register<WeakEnvelopePosintModel>(this, ShowEnvelopeMethod);
            this.PressMachineNo = pressMachineNo;
            PressMachineName = pressMachineName;
            if (plc != null)
            {
                Plc = plc;
            }

            AutoMesProperty.AutoModeID = pressMachineNo;

            CurveParaN = new CurvePara() { MinX = -20, MaxX = 200, MinY = 0, MaxY = 100 };
            var findPlotModel = PlotConfigManager.Instance[PressMachineName];
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (findPlotModel is not null)
                {
                    PlotModel = PlotHelper.CreatePlotModel(-20, 20, 0, 100, findPlotModel);
                }
                else
                {
                    PlotModel = PlotHelper.CreatePlotModel(-20, 20, 0, 100);
                }

                OxyColor testColor;
                if (findPlotModel is not null && !string.IsNullOrEmpty(findPlotModel.PlotColor))
                {
                    try
                    {
                        testColor = OxyColor.Parse(findPlotModel.PlotColor);
                    }
                    catch (Exception e)
                    {
                        testColor = OxyColors.Red;
                    }
                }
                else
                {
                    testColor = OxyColors.Red;
                }

                // 设置 PlotModel 的交互属性
                PlotModel.IsLegendVisible = true;
                var l = new Legend
                {
                    LegendPlacement = LegendPlacement.Inside,
                    LegendPosition = LegendPosition.LeftTop,
                    LegendOrientation = LegendOrientation.Vertical,
                    LegendBackground = OxyColors.Transparent,
                    LegendBorder = OxyColors.Transparent,

                    LegendItemAlignment = OxyPlot.HorizontalAlignment.Left,
                };
                PlotModel.Legends.Add(l);
                // 配置曲线
                _rawLineSeries = new LineSeries
                {
                    Title = "曲线",
                    TrackerFormatString = "X: {2:0.000}\nY: {4:0.000}",
                    Color = testColor,
                    StrokeThickness = 1.5,
                    InterpolationAlgorithm = _interpolationAlgorithm,
                    //InterpolationAlgorithm = new CatmullRomSpline(0.1)
                };

                PlotModel.Series.Add(_rawLineSeries);
            });


            //InitTestData();
        }

        public static readonly IInterpolationAlgorithm _interpolationAlgorithm
            //= new CatmullRomSpline(1);
            = null;
        // = InterpolationAlgorithms.UniformCatmullRomSpline;

        private float[] xValue = [];
        private float[] yTopValue = [];
        private float[] yBottonValue = [];

        public bool OpenEnvelope { get; private set; }
        public bool OpenEnvelopeTop { get; private set; }
        public bool OpenEnvelopeBottom { get; private set; }

        LineSeries _EnvelopeLineTopSeries = new()
        {
            InterpolationAlgorithm = _interpolationAlgorithm,
            Title = "上限位",
            TrackerFormatString = "{1}: {2:000.000}\n{3}: {4:000.000}",
            Color = OxyColors.PaleVioletRed,
        };

        //LineSeries _EnvelopeLineBottomSeries = new()
        //{
        //    InterpolationAlgorithm = interpolationAlgorithm,
        //    Title = "下限位",
        //    TrackerFormatString = "{1}: {2:000.000}\n{3}: {4:000.000}",
        //    Color = OxyColors.PaleVioletRed,
        //};

        PolygonAnnotation polygonAnnotation = new PolygonAnnotation
        {
            Fill = OxyColor.FromAColor(50, OxyColors.OrangeRed),
            Stroke = OxyColors.Transparent,
            StrokeThickness = 2
        };

        PolygonAnnotation polygonAnnotationBackground = new PolygonAnnotation
        {
            Fill = OxyColor.FromAColor(50, OxyColors.GreenYellow),
            Stroke = OxyColors.Transparent,
            StrokeThickness = 2
        };

        private void ShowEvlp(EnvelopePosintModel value)
        {
            _EnvelopeLineTopSeries.Points.Clear();
            polygonAnnotation.Points.Clear();
            polygonAnnotationBackground.Points.Clear();

            if (!OpenEnvelope)
            {
                return;
            }

            if (PlotModel is not null)
            {
                if (!PlotModel.Annotations.Contains(polygonAnnotation))
                {
                    PlotModel.Annotations.Add(polygonAnnotation);
                }

                if (!PlotModel.Annotations.Contains(polygonAnnotationBackground))
                {
                    PlotModel.Annotations.Add(polygonAnnotationBackground);
                }

                if (!PlotModel.Series.Contains(_EnvelopeLineTopSeries))
                {
                    PlotModel.Series.Add(_EnvelopeLineTopSeries);
                }
                //PlotModel?.Series.Add(_EnvelopeLineBottomSeries);
            }

            var value1 = value.Posints[0];
            polygonAnnotationBackground.Points.Add(new DataPoint(value1.X1, 0));
            foreach (var item in value.Posints)
            {
                var points = new DataPoint(item.X1, item.Y1);
                polygonAnnotation.Points.Add(points);
                polygonAnnotationBackground.Points.Add(new DataPoint(item.X1, item.Y2));
                if (OpenEnvelopeTop)
                {
                    _EnvelopeLineTopSeries.Points.Add(points);
                }
                //_EnvelopeLineBottomSeries.Points.Add(new DataPoint(item.X1, item.Y2));
            }

            if (OpenEnvelopeBottom)
            {
                foreach (var item in value.Posints.Reverse())
                {
                    var points = new DataPoint(item.X1, item.Y2);
                    polygonAnnotation.Points.Add(points);
                }
            }


            var value2 = value.Posints[value.Posints.Count - 1];
            polygonAnnotationBackground.Points.Add(new DataPoint(value2.X1, 0));


            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                PlotModel!.InvalidatePlot(true);
                PlotModel!.InvalidatePlot(false);
            });
        }

        private void ShowEnvelopeMethod(object recipient, WeakEnvelopePosintModel message)
        {
            _EnvelopeLineTopSeries.Points.Clear();
            //_EnvelopeLineBottomSeries.Points.Clear();
            polygonAnnotation.Points.Clear();
            polygonAnnotationBackground.Points.Clear();
            if (message.Status == WeakEnvelopePosintModelStatus.Hide)
            {
                PlotModel?.Annotations.Remove(polygonAnnotation);
                PlotModel?.Annotations.Remove(polygonAnnotationBackground);
                PlotModel?.Series.Remove(_EnvelopeLineTopSeries);
                //PlotModel?.Series.Remove(_EnvelopeLineBottomSeries);
            }
            else if (message.Status == WeakEnvelopePosintModelStatus.Show)
            {
                if (PlotModel is not null && !PlotModel.Annotations.Contains(polygonAnnotation))
                {
                    PlotModel?.Annotations.Add(polygonAnnotation);
                    PlotModel?.Annotations.Add(polygonAnnotationBackground);
                    PlotModel?.Series.Add(_EnvelopeLineTopSeries);
                    //PlotModel?.Series.Add(_EnvelopeLineBottomSeries);
                }

                var value1 = message.Value.Posints[0];
                polygonAnnotationBackground.Points.Add(new DataPoint(value1.X1, 0));
                foreach (var item in message.Value.Posints)
                {
                    var points = new DataPoint(item.X1, item.Y1);
                    polygonAnnotation.Points.Add(points);
                    polygonAnnotationBackground.Points.Add(new DataPoint(item.X1, item.Y2));
                    _EnvelopeLineTopSeries.Points.Add(points);
                    //_EnvelopeLineBottomSeries.Points.Add(new DataPoint(item.X1, item.Y2));
                }

                foreach (var item in message.Value.Posints.Reverse())
                {
                    var points = new DataPoint(item.X1, item.Y2);
                    polygonAnnotation.Points.Add(points);
                }

                var value2 = message.Value.Posints[message.Value.Posints.Count - 1];
                polygonAnnotationBackground.Points.Add(new DataPoint(value2.X1, 0));
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                PlotModel!.InvalidatePlot(true);
                PlotModel!.InvalidatePlot(false);
            });
        }

        private AutoMesProperties? _autoMesProperties;

        public AutoMesProperties AutoMesProperty
        {
            get { return _autoMesProperties ??= new AutoMesProperties(); }
            set { _autoMesProperties = value; }
        }

        // 初始化测试数据
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

        public void ChangeParamOxyPlotView(CurvePara? CurvePara,
            ObservableCollection<MonitorRec>? monitorRecs, string name, EnvelopePosintModel? envelopePosintModel = null)
        {
            if (CurvePara is null)
                return;

            AutoMesProperty.Recipe = name;

            if (envelopePosintModel is not null && AppSettings.Default.OpenEnvelope)
            {
                Envelope = envelopePosintModel;
                OpenEnvelope = envelopePosintModel.IsSelected;
                OpenEnvelopeTop = envelopePosintModel.Top;
                OpenEnvelopeBottom = envelopePosintModel.Bottom;
                xValue = envelopePosintModel.Posints.Select(e => e.X1).ToArray();
                if (envelopePosintModel.Top)
                {
                    yTopValue = envelopePosintModel.Posints.Select(e => e.Y1).ToArray();
                }
                else
                {
                    yTopValue = [];
                }

                if (envelopePosintModel.Bottom)
                {
                    yBottonValue = envelopePosintModel.Posints.Select(e => e.Y2).ToArray();
                }
                else
                {
                    yBottonValue = [];
                }
            }
            else
            {
                Envelope = new EnvelopePosintModel();
                OpenEnvelope = false;
                xValue = [];
                yBottonValue = [];
                yBottonValue = [];
            }

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
            if (AppSettings.Default.OpenEnvelope)
            {
                ShowEvlp(Envelope);
            }
            
        }

        private PointAnnotation? _envelopeAnnotation = null;

        public Result CheckEnvelope(DataPoint point)
        {
            if (!AppSettings.Default.OpenEnvelope)
            {
                return Result.None;
            }
            if (!OpenEnvelope)
            {
                return Result.None;
            }

            var findIndex = OptimizedValueFinder.FindValueStatus(xValue, (float)point.X, out var index);
            if (findIndex == FindStatus.NotExists_OutOfRange)
            {
                return Result.None;
            }

            var result = Result.None;
            if (OpenEnvelopeTop)
            {
                if (findIndex != FindStatus.NotExists_InRange)
                {
                    if (yTopValue[index] < point.Y)
                    {
                        CreateEnvelope(point,"压力过大");
                        return Result.Ng;
                    }
                }
                else
                {
                    var point1 = new DataPoint(xValue[index - 1], yTopValue[index - 1]);
                    var point2 = new DataPoint(xValue[index], yTopValue[index]);
                    var pointResult = LinePositionChecker.GetPointPositionRelativeToLine(point1, point2, point);

                    if (pointResult == PointPosition.Above)
                    {
                        CreateEnvelope(point, "压力过大");
                        return Result.Ng;
                    }
                }
            }

            if (OpenEnvelopeBottom)
            {
                if (findIndex != FindStatus.NotExists_InRange)
                {
                    if (yBottonValue[index] > point.Y)
                    {
                        CreateEnvelope(point, "压力过小");
                        return Result.Ng;
                    }
                }
                else
                {
                    var point1 = new DataPoint(xValue[index - 1], yBottonValue[index - 1]);
                    var point2 = new DataPoint(xValue[index], yBottonValue[index]);
                    var pointResult = LinePositionChecker.GetPointPositionRelativeToLine(point1, point2, point);
                    if (pointResult == PointPosition.Below)
                    {
                        CreateEnvelope(point, "压力过小");
                        return Result.Ng;
                    }
                }
            }

            return result;
        }

        private void CreateEnvelope(DataPoint point,string text = "")
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _envelopeAnnotation = PlotHelper.CreateInflection(PlotModel!,
                    (float)point.X, (float)point.Y, true, "#EF08FC", "#FC0808", text);
            });
        }


        public void ChangeParamOxyPlotView()
        {
            CurvePara? curvePara = this.CurveParaN;
            PlotModel?.InvalidatePlot(true);
            var xAxis = PlotModel!.Axes.FirstOrDefault(a => a.Key == "HorizontalAxis");
            var yAxis = PlotModel.Axes.FirstOrDefault(a => a.Key == "VerticalAxis");
            if (curvePara is not null && xAxis != null && yAxis != null)
            {
                xAxis.Minimum = curvePara.MinX;
                xAxis.Maximum = curvePara.MaxX;
                yAxis.Minimum = curvePara.MinY;
                yAxis.Maximum = curvePara.MaxY;
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

        private AutoPlotValue? resultAutoPlotValue;

        private void SetReuslt(Result result)
        {
            if (resultAutoPlotValue == null)
            {
                resultAutoPlotValue = AutoPlotValues.FirstOrDefault(e => e.Name.Contains("结果"));
            }

            if (resultAutoPlotValue is null) return;
            resultAutoPlotValue.Value = result switch { Result.None => 0, Result.Ok => 1, Result.Ng => 2, _ => -1 };
        }

        #endregion

        public int AddPosLength { get; set; } // 插入长度
        public DeviceCommunication Plc { get; set; }

        [ObservableProperty] private string? _Code;

        /// <summary>
        /// 自动画曲线
        /// </summary>
        public async void Test(string codeBase = "未扫码", DeviceCommunication? plc = null,
            List<string>? partialList = null)
        {
            //PlotModel?.Series.Add(_rawLineSeries);
            if (codeBase is "等待扫码中" || string.IsNullOrEmpty(codeBase))
            {
                codeBase = "未扫码";
            }

            if (int.TryParse(Step, out var value))
            {
                codeBase = this.GetMainCode(value);
                partialList = this.GetPartialCode(value);
            }


            AutoMesProperty.Code = codeBase;
            AutoMesProperty.StartTime = DateTime.Now;

            DispatcherHelper.CheckBeginInvokeOnUI(() => { this.Code = codeBase; });

            if (plc != null)
            {
                Plc = plc;
            }

            if (Plc is null)
            {
                throw new ArgumentNullException(nameof(Plc), "PLC连接对象不能为空");
            }
            var eovelopeResult = Result.None;
            IsUsing = false;
            var index = true;
            var startTime = DateTime.Now;
            DateTime lastUpdatePlotTime = DateTime.Now;
            var recording = false;
            PointAnnotation inflectionAnno = null;
            bool dataChanged = false;
            float endPostions = -1;
            double nowSize = 100;
            while (true)
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
                        AutoMesProperty.FinalTime = DateTime.Now;

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
                                        SetReuslt(Result.Ng);
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

                            AutoMesProperty.MaxPre = MaxPre;
                            AutoMesProperty.FinalPre = EndPre;
                            AutoMesProperty.MaxPos = _baseData.Max(d => d.Position);

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

                                if (eovelopeResult == Result.Ng)
                                {
                                    totalRet = Result.Ng;
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
                                    PressMachineName = PressMachineName,
                                    Result = totalRet == Result.Ok ? "OK" : "NG",
                                });

                                AutoMesProperty.ResultString = totalRet == Result.Ok ? "OK" : "NG";
                                AutoMesProperty.ResultInt = totalRet == Result.Ok ? 1 : 2;

                                SetReuslt(totalRet);
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

                                AutoMesProperty.FilePath = filename;

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
                                    Operator = LoginAuthHelper.LoginUser?.UserName,
                                    CreateTime = DateTime.Now,
                                    Recipe = RecipeName,
                                    Reuslt = totalRet == Result.Ok ? "OK" : "NG",
                                    Data = $"配方:{RecipeName},电缸号：{PressMachineNo}",
                                    Step = this.Step,
                                    PartialCodeList = partialList is null
                                        ? ""
                                        : string.Join(ApplicationConfigConst.AutoModeJoinChar, partialList),
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
                                if (AutoMesConfigManager.Instance.AutoMesConfig.OpenAfterChecked)
                                    switch (AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesMode)
                                    {
                                        case AutoMesAfterMode.Sql:
                                            {
                                                var result =
                                                    await AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesSql
                                                        ?.ExecuteSaveSql(AutoMesProperty);
                                                if (!result.IsSuccess)
                                                {
                                                    Growl.ErrorGlobal("Mes数据上传失败！");
                                                }

                                                break;
                                            }
                                        case AutoMesAfterMode.Http:
                                            {
                                                var resultRes =
                                                    await AutoMesConfigManager.Instance.AutoMesConfig.AfterAutoMesHttp
                                                        ?.RequestBodyBoolResult(AutoMesProperty);
                                                if (!resultRes)
                                                {
                                                    Growl.ErrorGlobal("Mes数据上传失败！");
                                                }

                                                break;
                                            }
                                    }

                                await GarnetPublishPressMachineInfo();
                                // 清除结果状态                              
                                foreach (var rec in MonitorRecs)
                                {
                                    rec.Result = Result.None;
                                }
                            }

                            ;
                        }

                        IsUsing = true;
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            SnackbarHelper.Show($"{this._autoMesProperties?.AutoModeID}曲线记录完成");
                        });
                        return;
                    }

                    // 开始记录
                    if (index)
                    {
                        SetReuslt(Result.None);
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
                            PlotModel!.InvalidatePlot(true);
                            PlotHelper.RemoveInflection(PlotModel!, inflectionAnno!);
                            PlotHelper.RemoveInflection(PlotModel, _envelopeAnnotation);
                            
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
                            if (endPostions < pos.Content)
                            {
                                endPostions = (float)pos.Content;
                                if ((float)pre!.Content < 100000)
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
                                    var pon = new DataPoint(measurement.Position,
                                            measurement.Pressure);
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        _rawLineSeries.Points.Add(pon);
                                        RawData.Add(measurement);
                                    });
                                    //if (_baseData.Count > 100000)
                                    //{
                                    //    _baseData.RemoveAt(0);
                                    //    _rawLineSeries.Points.RemoveAt(0);
                                    //    RawData.RemoveAt(0);
                                    //}
                                    if (eovelopeResult != Result.Ng)
                                    {
                                        eovelopeResult = CheckEnvelope(pon);
                                        if (eovelopeResult == Result.Ng)
                                        {
                                            WeakReferenceMessenger.Default.Send(new PressMacchineResultWeak
                                            {
                                                InflectionPos = 0,
                                                InflectionPre = 0,
                                                Token = PressMachineNo,
                                                Result = "NG",
                                            });
                                            SetReuslt(Result.Ng);
                                            var alarmRet = Plc?.Write(PressRes, (ushort)2);
                                        }
                                    }


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
                                                SetReuslt(Result.Ng);
                                                var alarmRet = Plc?.Write(PressRes, (ushort)2);
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

                    Thread.Sleep(10);
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

        private ConnectionMultiplexer? _garnetClient;


        /// <summary>
        /// 在有序数组中查找指定值的插入位置索引
        /// </summary>
        /// <param name="sortedArray">有序数组（假设已按升序排列）</param>
        /// <param name="value">要插入的值</param>
        /// <returns>插入位置的索引，若值超出数组范围则返回-1</returns>
        public static int FindInsertIndex(double[] sortedArray, double value)
        {
            // 处理空数组或数组长度为0的情况
            if (sortedArray == null || sortedArray.Length == 0)
                return -1;

            // 检查值是否超出数组范围
            if (value < sortedArray[0] || value > sortedArray[sortedArray.Length - 1])
                return -1;

            // 遍历数组查找插入位置
            for (int i = 0; i < sortedArray.Length; i++)
            {
                // 找到第一个大于等于目标值的元素位置
                if (sortedArray[i] >= value)
                {
                    // 如果找到的元素等于目标值，返回当前索引
                    if (Math.Abs(sortedArray[i] - value) < 1e-9)
                        return i;
                    // 否则返回当前索引（插入到该元素之前）
                    return i;
                }
            }

            // 理论上不会执行到这里，因为前面已检查值是否超出范围
            return -1;
        }


        private async Task GarnetPublishPressMachineInfo()
        {
            if (AppSettings.Default is not null && AppSettings.Default.OpenGarnet)
            {
                _garnetClient ??= await GarnetClient.ConnectGarnetAsync(ApplicationConfigConst.GarnetConnectString);

                var stringValue = System.Text.Json.JsonSerializer.Serialize(this._autoMesProperties);
                var db = _garnetClient.GetDatabase(8);
                db.StringSet("PressMachineResultInfo", stringValue);

                var publicGarnet = _garnetClient.GetSubscriber();
                await publicGarnet.PublishAsync(RedisChannel.Literal("PressMachineResultInfo"),
                    stringValue);
            }
        }
    }
}