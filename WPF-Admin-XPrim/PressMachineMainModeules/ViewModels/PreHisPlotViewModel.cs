using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PressMachineMainModeules.Config;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using PressMachineMainModeules.Views;
using System.IO;
using WPF.Admin.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Helper;
using WPF.Admin.Themes.I18n;

namespace PressMachineMainModeules.ViewModels
{


    public partial class PreHisPlotViewModel : BindableBase
    {
        [ObservableProperty] private string _title = nameof(PreHisPlotViewModel);
        [ObservableProperty]
        private PlotModel _PlotModel;
        [ObservableProperty]
        private string _PathFile = "";

        public PreHisPlotViewModel()
        {
            (t, _) = CSharpI18n.UseI18n();
            var findPlotModel = PlotConfigManager.Instance.PlotConfigModels.FirstOrDefault();
            PlotModel = PlotHelper.CreatePlotModel(0, 300, 0, 300,findPlotModel!);
            PlotThemesHelper.SetThemePlot(PlotModel);

        }

        public void LoadingPlot(string fullFileName)
        {
            if (PathFile == fullFileName) return;


            PathFile = fullFileName;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                PreHisPlot.XioaMasks.Loading(() =>
                {

                    Thread.Sleep(2000);

                    try
                    {
                        FileInfo info = new FileInfo(fullFileName);
                        _file = info;
                        LoadData();

                    }
                    catch (Exception ex)
                    {
                        Growl.ErrorGlobal(ex.Message);
                    }
                });
            });

        }

        private FileInfo _file;
        [RelayCommand]
        private void SelectMore()
        {
            var files = new List<FileInfo>();

            try
            {

                using var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    Multiselect = true,
                    Title = t!("History.SelectFile"),
                    InitialDirectory = Common.DataFolder
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)

                {
                    var file = openFileDialog.FileNames;

                    foreach (var f in file)
                    {
                        FileInfo info = new FileInfo(f);
                        files.Add(info);
                    }
                    //PathFile = file;
                    //FileInfo info = new FileInfo(file);
                    //_file = info;
                    PreHisPlot.XioaMasks.Loading(async() =>
                    {
                       await LoadData(files);
                    });


                }
                PathFile = t!("History.MultiFileLoading");

            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal(ex.Message);
            }

        }


        [RelayCommand]
        private void Select()
        {


            try
            {
                using var openFileDlg = new System.Windows.Forms.OpenFileDialog
                {
                    DefaultExt = ".xlsx",
                    Filter = "Test files (.xlsx)|*.xlsx|All files|*.*",
                    Multiselect = false,
                    Title = t!("History.SelectTestFile"),
                    InitialDirectory = Common.DataFolder
                };
                if (openFileDlg.ShowDialog() == DialogResult.OK)
                {
                    var file = openFileDlg.FileName;
                    PathFile = file;
                    FileInfo info = new FileInfo(file);
                    _file = info;
                    PreHisPlot.XioaMasks.Loading(() =>
                    {
                        LoadData();
                    });
                }
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal(ex.Message);
            }



        }


        public AnalysisData analysisData { get; set; }

        private async Task LoadData(List<FileInfo> fileInfos)
        {
            try
            {
                var datas = new List<AnalysisData>();
                foreach (FileInfo file in fileInfos)
                {
                    DataExcelHelper.Read(
                          file.FullName,
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
                    datas.Add(new AnalysisData
                    {
                        FileFullName = file.FullName,
                        Recs = recs,
                        CurvePara = curvePara,
                        FittedPostions = fittedPositions,
                        FittedPressures = fittedPressures,
                        InflectionPos = inflectionPos,
                        InflectionPre = inflectionPre
                    });
                }
                DispatcherHelper.CheckBeginInvokeOnUI( async() =>
                {
                   await ShowMore(datas);
                });

            }
            catch (Exception ex)
            {
                SnackbarHelper.Show(ex.Message);
            }

        }
        private async void LoadData()
        {
            var fileName = _file.FullName;
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
                Growl.ErrorGlobal($"{t!("History.FileError")}!{ex.Message}");
            }



        }

        private async Task ShowMore(List<AnalysisData> datas)
        {
            PlotModel.Series.Clear();
            PlotModel.Annotations.Clear();
            PlotModel.InvalidatePlot(true);
            PlotModel.InvalidatePlot(false);

            PlotModel.Annotations.Clear();
            var _xAxis = (LinearAxis)PlotModel!.Axes.FirstOrDefault(a => a.Key == "HorizontalAxis");
            var _yAxis = (LinearAxis)PlotModel.Axes.FirstOrDefault(a => a.Key == "VerticalAxis");

            foreach (var a in datas)
            {
                LineSeries series = new LineSeries()
                {
                    TrackerFormatString = "{1}: {2:000.000}\n{3}: {4:000.000}",
                    Color = OxyColors.Orange,
                };
                PlotModel.Series.Add(series);
                var fittedPositions = a.FittedPostions;
                var fittedPressures = a.FittedPressures;
                for (int i = 0; i < fittedPositions.Length; i++)
                {
                    series.Points.Add(new DataPoint(fittedPositions[i], fittedPressures[i]));
                }
                Thread.Sleep(150);
            }



            PlotModel.InvalidatePlot(true);
        }


        private void Show()
        {
            PlotModel.Series.Clear();
            PlotModel.Annotations.Clear();
            PlotModel.InvalidatePlot(true);
            PlotModel.InvalidatePlot(false);

            var recs = analysisData.Recs;
            var curvePara = analysisData.CurvePara;
            var fittedPositions = analysisData.FittedPostions;
            var fittedPressures = analysisData.FittedPressures;

            PlotModel.Annotations.Clear();

            LineSeries series = new LineSeries()
            {
                TrackerFormatString = "{1}: {2:000.000}\n{3}: {4:000.000}",
                Color = OxyColors.Orange,
            }; PlotModel.Series.Add(series);
            PlotHelper.CreateMonitorRec(PlotModel, recs);
            var _xAxis = (LinearAxis)PlotModel!.Axes.FirstOrDefault(a => a.Key == "HorizontalAxis");
            var _yAxis = (LinearAxis)PlotModel.Axes.FirstOrDefault(a => a.Key == "VerticalAxis");
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (_xAxis != null && _yAxis != null)
                {
                    if (curvePara.MinX != curvePara.MaxX)
                    {
                        _xAxis.Minimum = curvePara.MinX;
                        _xAxis.Maximum = curvePara.MaxX;
                    }
                    else
                    {
                        _xAxis.Minimum = fittedPositions.Min() - 10;
                        _xAxis.Maximum = fittedPositions.Max() + 10;
                    }
                    if (curvePara.MinY != curvePara.MaxY)
                    {
                        _yAxis.Minimum = curvePara.MinY;
                        _yAxis.Maximum = curvePara.MaxY;
                    }
                    else
                    {
                        _yAxis.Minimum = fittedPressures.Min() - 10;
                        _yAxis.Maximum = fittedPressures.Max() + 10;
                    }

                }
                PlotModel.ResetAllAxes();
                for (int i = 0; i < fittedPositions.Length; i++)
                {
                    series.Points.Add(new DataPoint(fittedPositions[i], fittedPressures[i]));
                }
                PlotModel.InvalidatePlot(true);
            });
        }


        [RelayCommand]
        private void Print()
        {
            PrintView printView = new PrintView();
            if(printView.DataContext is PrintViewModel vm)
            {              
                vm.SetView(analysisData);
            }
            printView.ShowDialog();
        }
    }
}
