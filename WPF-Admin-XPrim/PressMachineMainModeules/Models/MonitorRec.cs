
namespace PressMachineMainModeules.Models
{
    /// <summary>
    /// 判断参数
    /// </summary>
    [Serializable]
    public class MonitorRec
    {
        public float Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }
        public Result Result { get; set; } = Result.None;
        public MonitorType MonitorType { get; set; } = MonitorType.BottomInBottomOut;
        public AlarmReaction AlarmReaction { get; set; }

        public static void Judge(IList<Measurement> data, MonitorRec rec)
        {
            // 根据矩形框位置上下限筛选数据
            var temp = data.Where(d => d.Position <= rec.MaxX && d.Position >= rec.MinX).ToList();
            if (temp.Count > 0)
            {
                // 根据矩形框压力上下限筛选数据
                var dataInRecByPressure = temp.Where(d => d.Pressure >= rec.MinY && d.Pressure <= rec.MaxY).ToList();
                if (dataInRecByPressure.Count > 0)
                {
                    // 根据压力取左位置和右位置
                    var lefPosition = dataInRecByPressure.Min(d => d.Position);
                    var rightPosition = dataInRecByPressure.Max(d => d.Position);

                    var leftPoints = temp.Where(d => d.Position < lefPosition).ToList();
                    var midPoints = temp.Where(d => d.Position >= lefPosition && d.Position <= rightPosition).ToList();
                    var rightPoints = temp.Where(d => d.Position > rightPosition).ToList();

                    switch (rec.MonitorType)
                    {
                        case MonitorType.BottomInRightOut:

                            if (leftPoints.Count > 0
                                && leftPoints.All(d => d.Pressure < rec.MinY)
                                && midPoints.Count > 0
                                && midPoints.All(d => d.Pressure >= rec.MinY && d.Pressure <= rec.MaxY)
                                && rightPoints.Count < 1
                                && data.Any(d => d.Position > rec.MaxX))
                            {
                                rec.Result = Result.Ok;
                                return;
                            }
                            break;
                        case MonitorType.BottomInBottomOut:

                            if (leftPoints.Count > 0
                                && leftPoints.All(d => d.Pressure < rec.MinY)
                                && midPoints.Count > 0
                                && midPoints.All(d => d.Pressure >= rec.MinY && d.Pressure <= rec.MaxY)
                                && rightPoints.Count > 0
                                && rightPoints.All(d => d.Pressure < rec.MinY))
                            {
                                rec.Result = Result.Ok;
                                return;
                            }
                            break;
                        case MonitorType.LeftInRightOut:
                            if (leftPoints.Count < 1
                                && midPoints.Count > 0
                                && midPoints.All(d => d.Pressure >= rec.MinY && d.Pressure <= rec.MaxY)
                                && rightPoints.Count < 1
                                && data.Any(d => d.Position < rec.MinX)
                                && data.Any(d => d.Position > rec.MaxX))
                            {
                                rec.Result = Result.Ok;
                                return;
                            }
                            break;
                        case MonitorType.LeftInTopOut:
                            rightPoints = temp.Where(d => d.Position > rightPosition).ToList();
                            if (leftPoints.Count < 1
                                && midPoints.Count > 0
                                && midPoints.All(d => d.Pressure >= rec.MinY && d.Pressure <= rec.MaxY)
                                && rightPoints.Count > 0
                                && rightPoints.All(d => d.Pressure > rec.MaxY)
                                && data.Any(d => d.Position < rec.MinX))
                            {
                                rec.Result = Result.Ok;
                                return;
                            }
                            break;
                        case MonitorType.BottomInNoOut:
                            {
                                var max = decimal.Parse(data.Max(e => e.Pressure).ToString());
                                if (max >= decimal.Parse(rec.MaxY.ToString()))
                                    break;
                                if (max <= decimal.Parse(rec.MinY.ToString()))
                                    break;

                                //temp = data.Where(d => d.Position <= rec.MaxX && d.Position >= rec.MinX).ToList();
                                //if (temp.Max(e => e.Pressure >= rec.MaxY))
                                //    break;
                                leftPoints = temp.Where(d => d.Position < lefPosition).ToList();
                                if (leftPoints.Count > 0
                                    && leftPoints.All(d => d.Pressure < rec.MinY)
                                    && midPoints.Count > 0
                                    && midPoints.All(d => d.Pressure >= rec.MinY && d.Pressure <= rec.MaxY)
                                    && rightPoints.Count < 1
                                    && data.All(d => d.Position < rec.MaxX))
                                {
                                    rec.Result = Result.Ok;
                                    return;
                                }
                                break;
                            }
                        case MonitorType.LeftInRightNoOut:
                            if (data.Any(d => d.Position < rec.MinX)
                                && data.Any(d => d.Position >= rec.MinX)
                                && data.All(d => d.Position < rec.MaxX))
                            {
                                if (data.Where(d => d.Position >= rec.MinX).All(d => d.Pressure <= rec.MaxY && d.Pressure >= rec.MinY))
                                {
                                    rec.Result = Result.Ok;
                                }
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                switch (rec.MonitorType)
                {
                    case MonitorType.BottomInRightOut:
                        break;
                    case MonitorType.BottomInBottomOut:
                        break;
                    case MonitorType.LeftInRightOut:

                        var minP = data.FirstOrDefault(pp => pp.Position <= rec.MinX);
                        var maxP = data.FirstOrDefault(pp => pp.Position >= rec.MaxX);
                        if (minP != null && maxP != null)
                        {

                            var para = MathNet.Numerics.Fit.Line(
                                new double[] { minP.Position, maxP.Position },
                                new double[] { minP.Pressure, maxP.Pressure });
                            var pre1 = para.Item1 + rec.MinX * para.Item2;
                            var pre2 = para.Item1 + rec.MaxX * para.Item2;
                            if (pre1 <= rec.MaxY && pre1 >= rec.MinY && pre2 <= rec.MaxY && pre2 >= rec.MinY)
                            {
                                rec.Result = Result.Ok;
                                return;
                            }
                        }
                        break;
                    case MonitorType.LeftInTopOut:
                        break;
                    case MonitorType.BottomInNoOut:
                        break;
                    case MonitorType.LeftInRightNoOut:
                        break;
                    default:
                        break;
                }
            }
            rec.Result = Result.Ng;
            return;
        }
    }
}
