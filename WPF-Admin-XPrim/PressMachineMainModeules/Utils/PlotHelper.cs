using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot;
using PressMachineMainModeules.Models;
using WPF.Admin.Models.Models;
using PressMachineMainModeules.ViewModels;

namespace PressMachineMainModeules.Utils
{
    public class PlotHelper
    {
        public static PlotModel CreatePlotModel(double minX, double maxX, double minY, double maxY,
            PlotConfigModel plotConfigModel)
        {
            return CreatePlotModel(minX, maxX, minY, maxY,
                plotConfigModel.IsZoom,
                plotConfigModel.IsPan,
                plotConfigModel.YName,
                plotConfigModel.YUnit,
                plotConfigModel.IsZoom,
                plotConfigModel.IsPan,
                plotConfigModel.XName,
                plotConfigModel.XUnit);
        }


        public static PlotModel CreatePlotModel(double minX, double maxX, double minY, double maxY,
            bool isZommY = false, bool isPanY = false, string? yName = "", string? yUnit = "",
            bool isZommX = false, bool isPanX = false, string? xName = "", string? xUnit = ""
        )
        {
            if (ApplicationAuthTaskFactory.AuthFlag)
            {
                throw new Exception("授权失败，无法获取UI");
            }

            if (string.IsNullOrEmpty(yName))
            {
                yName = PressMachineParamsViewModel.PressMachineParam.YName;
            }

            if (string.IsNullOrEmpty(yUnit))
            {
                yUnit = PressMachineParamsViewModel.PressMachineParam.YUnit;
            }

            if (string.IsNullOrEmpty(xName))
            {
                xName = PressMachineParamsViewModel.PressMachineParam.XName;
            }

            if (string.IsNullOrEmpty(xUnit))
            {
                xUnit = PressMachineParamsViewModel.PressMachineParam.XUnit;
            }


            var model = new PlotModel();
            var verticalAxis = new LinearAxis
            {
                IsZoomEnabled = isZommY, IsPanEnabled = isPanY, Position = AxisPosition.Left, Minimum = minX,
                Maximum = maxX, Key = "VerticalAxis",
                Unit = $"{yName}-{yUnit}"
            };
            model.Axes.Add(verticalAxis);
            model.Axes.Add(new LinearAxis
            {
                IsZoomEnabled = isZommX, IsPanEnabled = isPanX, Position = AxisPosition.Bottom, Minimum = minY,
                Maximum = maxY, Key = "HorizontalAxis",
                Unit = $"{xName}-{xUnit}"
            });
            model.InvalidatePlot(false);
            return model;
        }

        public static void CreateMonitorRec(PlotModel model, List<MonitorRec> recs)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (recs == null)
            {
                throw new ArgumentNullException("recs");
            }

            foreach (var rec in recs)
            {
                LineAnnotation line1 = new LineAnnotation
                {
                    X = rec.MinX,
                    MinimumY = rec.MinY,
                    MaximumY = rec.MaxY,
                    Color = OxyColors.Red,
                    LineStyle = LineStyle.Solid,
                    Type = LineAnnotationType.Vertical,
                    StrokeThickness = 1.5
                };

                LineAnnotation line2 = new LineAnnotation
                {
                    X = rec.MaxX,
                    MinimumY = rec.MinY,
                    MaximumY = rec.MaxY,
                    Color = OxyColors.Red,
                    LineStyle = LineStyle.Solid,
                    Type = LineAnnotationType.Vertical,
                    StrokeThickness = 1.5
                };

                LineAnnotation line3 = new LineAnnotation
                {
                    Y = rec.MinY,
                    MinimumX = rec.MinX,
                    MaximumX = rec.MaxX,
                    Color = OxyColors.Red,
                    LineStyle = LineStyle.Solid,
                    Type = LineAnnotationType.Horizontal,
                    StrokeThickness = 1.5
                };

                LineAnnotation line4 = new LineAnnotation
                {
                    Y = rec.MaxY,
                    MinimumX = rec.MinX,
                    MaximumX = rec.MaxX,
                    Color = OxyColors.Red,
                    LineStyle = LineStyle.Solid,
                    Type = LineAnnotationType.Horizontal,
                    StrokeThickness = 1.5
                };
                model.Annotations.Add(line1);
                model.Annotations.Add(line2);
                model.Annotations.Add(line3);
                model.Annotations.Add(line4);

                var rectangle = new RectangleAnnotation();
                rectangle.MinimumX = rec.MinX;
                rectangle.MaximumX = rec.MaxX;
                rectangle.MinimumY = rec.MinY;
                rectangle.MaximumY = rec.MaxY;
                rectangle.Fill = OxyColor.FromAColor(50, OxyColors.LightGreen);
                rectangle.Text = rec.Name;
                rectangle.TextPosition = new DataPoint(rec.MinX, rec.MinY);

                model.Annotations.Add(rectangle);

                switch (rec.MonitorType)
                {
                    case MonitorType.BottomInRightOut:
                        line2.Color = OxyColors.Green;
                        line2.LineStyle = LineStyle.Dash;
                        line3.Color = OxyColors.Green;
                        line3.LineStyle = LineStyle.Dash;
                        break;
                    case MonitorType.BottomInBottomOut:
                        line3.Color = OxyColors.Green;
                        line3.LineStyle = LineStyle.Dash;
                        break;
                    case MonitorType.LeftInRightOut:
                        line1.Color = OxyColors.Green;
                        line1.LineStyle = LineStyle.Dash;
                        line2.Color = OxyColors.Green;
                        line2.LineStyle = LineStyle.Dash;
                        break;
                    case MonitorType.LeftInTopOut:
                        line1.Color = OxyColors.Green;
                        line1.LineStyle = LineStyle.Dash;
                        line4.Color = OxyColors.Green;
                        line4.LineStyle = LineStyle.Dash;
                        break;
                    case MonitorType.BottomInNoOut:
                        line3.Color = OxyColors.Green;
                        line3.LineStyle = LineStyle.Dash;
                        break;
                    case MonitorType.LeftInRightNoOut:
                        line1.Color = OxyColors.Green;
                        line1.LineStyle = LineStyle.Dash;

                        break;
                    default:
                        break;
                }
            }

            model.InvalidatePlot(false);
        }

        public static void CreateInflectionRec(PlotModel model, float minX, float maxX, float minY, float maxY)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            RectangleAnnotation rec = new RectangleAnnotation
            {
                MinimumX = minX,
                MaximumX = maxX,
                MinimumY = minY,
                MaximumY = maxY,
                Stroke = OxyColors.Green,
                StrokeThickness = 1.5,
                Fill = OxyColors.LightGray,
                ToolTip = "限定拐点范围的拐点框",
                Text = "拐点框",
                TextPosition = new DataPoint(minX, minY),
                Layer = AnnotationLayer.BelowSeries
            };
            model.Annotations.Add(rec);

            model.InvalidatePlot(false);
        }

        public static PointAnnotation CreateInflection(PlotModel model, float x, float y, bool updateData = false,
            string? colorStroke = null, string? colorFill = null,string text = "")
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }


            PointAnnotation p = new PointAnnotation
            {
                Shape = MarkerType.Circle,
                Size = 6,
                Stroke = OxyColors.Red,
                StrokeThickness = 1,
                Fill = OxyColors.LightBlue,
                Text = text,
                X = x,
                Y = y,
                Layer = AnnotationLayer.BelowSeries,
            };

            if (colorStroke is not null)
            {
                p.Stroke = OxyColor.Parse(colorStroke);
            }
            
            if (colorFill is not null)
            {
                p.Fill = OxyColor.Parse(colorFill);
            }
            

            model.Annotations.Add(p);

            model.InvalidatePlot(updateData);

            return p;
        }

        public static void RemoveInflection(PlotModel? model, PointAnnotation? p)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (p == null || !model.Annotations.Contains(p))
            {
                return;
            }

            model.Annotations.Remove(p);
            model.InvalidatePlot(false);
        }
    }
}