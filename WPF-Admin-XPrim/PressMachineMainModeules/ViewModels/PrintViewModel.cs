using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PressMachineMainModeules.Models;
using PressMachineMainModeules.Utils;
using System.Windows.Media;
using WPF.Admin.Models;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Helper;

namespace PressMachineMainModeules.ViewModels
{
    public partial class PrintViewModel : BindableBase
    {
        [ObservableProperty]
        private PlotModel _plotModel;
        [ObservableProperty]
        private string _ProductName;
        [ObservableProperty]
        private string _Code;
        [ObservableProperty]
        private string _Result;
        [ObservableProperty]
        private string _InflectionPos;
        [ObservableProperty]
        private string _InflectionPre;
        [ObservableProperty]
        private DateTime _PrintDateTime;
        public PrintViewModel()
        {
            PlotModel = PlotHelper.CreatePlotModel(0, 300, 0, 300);
        }

        [RelayCommand]
        private void Print(Visual visual)
        {
            PrintXPSHelper.PrintXPS(visual);
        }

        public void SetInformation(PrintInfomation dto)
        {
            
        }

        public void SetView(AnalysisData? analysisData)
        {
            if(analysisData is null) return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.Code = analysisData.Code;
                this.Result = analysisData.Ret.ToString();
                this.ProductName = analysisData.ProductName;
                this.InflectionPos = analysisData.InflectionPos.ToString(CultureInfo.InvariantCulture);
                this.InflectionPre = analysisData.InflectionPre.ToString(CultureInfo.InvariantCulture);
                this.PrintDateTime = DateTime.Now;
            });


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
    }

    public class PrintInfomation
    {
        public string InflectionPre { get; set; } = "99999";
        public string InflectionPos { get; set; } = "99999";
        public string Result { get; set; } = "无结果";
        public string ProductName { get; set; } = "未命名";
        public string Code { get; set; } = "未扫码";
    }
}
