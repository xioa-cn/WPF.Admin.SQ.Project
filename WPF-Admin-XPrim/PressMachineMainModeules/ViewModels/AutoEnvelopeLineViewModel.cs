using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Helper;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services;

namespace PressMachineMainModeules.ViewModels
{
    public partial class AutoEnvelopeLineViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private static AutoEnvelopeLineViewModel? autoEnvelopeLineViewModel;

        public static AutoEnvelopeLineViewModel Instance
        {
            get
            {
                return autoEnvelopeLineViewModel ??= new AutoEnvelopeLineViewModel();
            }
        }

        public AutoEnvelopeLineViewModel()
        {
            PlotModel = PlotHelper.CreatePlotModel(-20, 20, 0, 100,
                isZommY: true, isPanY: true, isZommX: true, isPanX: true);
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
           
        }
       
        


        [ObservableProperty] private float _offsetValue;
        [ObservableProperty] private float _interpolationValue;
        [ObservableProperty] private PlotModel? _plotModel;

        private FileInfo _file;

        [RelayCommand]
        public void OpenFileDialogSelectExcel()
        {
            try
            {
                using var openFileDlg = new System.Windows.Forms.OpenFileDialog
                {
                    DefaultExt = ".xlsx",
                    Filter = "Test files (.xlsx)|*.xlsx|All files|*.*",
                    Multiselect = false,
                    Title = "选择测试文件",
                    InitialDirectory = Common.DataFolder
                };
                if (openFileDlg.ShowDialog() == DialogResult.OK)
                {
                    var file = openFileDlg.FileName;
                    FileInfo info = new FileInfo(file);
                    _file = info;

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal(ex.Message);
            }
        }
        public AnalysisData analysisData { get; set; }
        private void LoadData()
        {
            var fileName = _file?.FullName;

            try
            {
                DataExcelHelper.Read(
                       fileName,
                       out string productName,
                       out string code,
                       out Result ret,
                       out float inflectionPos,
                       out float inflectionPre,
                       out List<MonitorRec> recs,
                       out CurvePara curvePara,
                       out double[] fittedTimes,
                       out double[] fittedPositions,
                       out double[] fittedPressures);
                analysisData = new AnalysisData
                {
                    FileFullName = _file.FullName,
                    Recs = recs,
                    CurvePara = curvePara,
                    FittedPostions = fittedPositions,
                    FittedPressures = fittedPressures,
                    InflectionPos = inflectionPos,
                    InflectionPre = inflectionPre,
                    ProductName = productName,
                    Code = code,
                    Ret = ret
                };
                Show();
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"文件加载失败!{ex.Message}");
            }
        }

        LineSeries _EnvelopeLineTopSeries = new()
        {
            InterpolationAlgorithm = AutoPlotViewModel._interpolationAlgorithm,
            Title = "上限位",
            TrackerFormatString = "{1}: {2:000.000}\n{3}: {4:000.000}",
            Color = OxyColors.PaleVioletRed,

        };

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
        [ObservableProperty]
        private ObservableCollection<PosintModel> _Posints = new ObservableCollection<PosintModel>();

        [RelayCommand]
        private void Show()
        {
            if (_interpolationValue == 0)
            {
                InterpolationValue = 10;
            }


            var xValue = analysisData.FittedPostions
                .Select((value, index) => new { value, index })
                .Where(item => item.index % _interpolationValue == 0)
                .Select(item => item.value > 0 ? item.value : 0)
                .ToList();

            var yValue = analysisData.FittedPressures
                .Select((value, index) => new { value, index })
                .Where(item => item.index % _interpolationValue == 0)
                .Select(item => item.value > 0 ? item.value : 0)
                .ToList();

            Posints = new ObservableCollection<PosintModel>(
                xValue.Zip(yValue, (x, y) => new PosintModel
                {
                    X1 = (float)x,
                    Y1 = (float)y + OffsetValue,
                    Y2 = (float)y - OffsetValue
                }).ToList());
            UpdatePlotModel();
        }

        [RelayCommand]
        public async Task AddPosit()
        {
            var result =
                await PressMachineDialogHelper.ShowPosintDialog("请输入参数", HcDialogMessageToken.DialogAutoEnvelopeLineToken
                    , buttontype: MessageBoxButton.OKCancel);

            if (result.Item1 == MessageBoxResult.OK)
            {
                this.Posints.Add(result.Item2);
                OrderByValue();
            }
        }

        private void OrderByValue()
        {
            this.Posints = new ObservableCollection<PosintModel>(this.Posints.OrderBy(e => e.X1));
        }

        public void RemoveValue(PosintModel posintModel)
        {
            this.Posints.Remove(posintModel);
        }

        [RelayCommand]
        private void UpdatePlotModel()
        {
            if (Posints.Count < 2)
            {
                Growl.ErrorGlobal("数据点过少，无法生成包络线");
                return;
            }

            _EnvelopeLineTopSeries.Points.Clear();
            polygonAnnotation.Points.Clear();
            polygonAnnotationBackground.Points.Clear();


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
            }

            var value1 = Posints.First();
            polygonAnnotationBackground.Points.Add(new DataPoint(value1.X1, 0));
            foreach (var item in Posints)
            {
                var points = new DataPoint(item.X1, item.Y1);
                polygonAnnotation.Points.Add(points);
                DataPoint points1;
                if (item.Y2 < 0)
                {
                    points1 = new DataPoint(item.X1, 0);
                    item.Y2 = 0;
                }
                else
                {
                    points1 = new DataPoint(item.X1, item.Y2);
                }
                polygonAnnotationBackground.Points.Add(points1);
                _EnvelopeLineTopSeries.Points.Add(points);
            }


            foreach (var item in Posints.Reverse())
            {
                DataPoint points;
                if (item.Y2 < 0)
                {
                    item.Y2 = 0;
                    points = new DataPoint(item.X1, 0);
                }
                else
                {
                    points = new DataPoint(item.X1, item.Y2);
                }

                polygonAnnotation.Points.Add(points);
            }



            var value2 = Posints.Last();
            polygonAnnotationBackground.Points.Add(new DataPoint(value2.X1, 0));


            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                PlotModel!.InvalidatePlot(true);
                PlotModel!.InvalidatePlot(false);
            });

            Growl.SuccessGlobal("Success");
        }
    }
}
