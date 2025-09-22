using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils
{
    public static class DataGenerator
    {
        public static bool FitDataAndUpdate(
            IList<Measurement> data,
            out double[] fittedTimes,
            out double[] fittedPositions,
            out double[] fittedPressures)
        {
            var countlength = data.Count();
            fittedTimes = new double[countlength];

            for (int i = 0; i < countlength; i++)
            {
                fittedTimes[i] = (data[i].TimeStamp - data[0].TimeStamp).TotalSeconds;
            }

            fittedPositions = data.Select(e => (double)e.Position).ToArray();
            fittedPressures = data.Select(e => (double)e.Pressure).ToArray();
            return true;


            fittedTimes = new double[0];
            fittedPositions = new double[0];
            fittedPressures = new double[0];
            if (data != null && data.Count > 10)
            {
                // 判定为压机未动
                if (data.ToList().Max(m => m.Position) < 10)
                {
                    return false;
                }

                // Linear fit curve.
                int count = data.Count;
                double[] times = new double[count];
                double[] positions = new double[count];
                double[] pressures = new double[count];
                for (int i = 0; i < count; i++)
                {
                    times[i] = (data[i].TimeStamp - data[0].TimeStamp).TotalSeconds;
                    positions[i] = data[i].Position;
                    pressures[i] = data[i].Pressure;
                    //Debug.WriteLine($"Raw time: {times[i]}");
                }

                List<double> tempTimes = new List<double>();
                double fittedTime = 0;
                double timeSpan =Config. Common.InterpolationSpan / 1000.0f;
                for (int i = 0; fittedTime < times[count - 1]; i++)
                {
                    fittedTime = timeSpan * i;
                    tempTimes.Add(fittedTime);
                }

                fittedTimes = tempTimes.ToArray();
                fittedPositions = LineInterpolation.Interpolate(times, positions, fittedTimes);
                fittedPressures = LineInterpolation.Interpolate(positions, pressures, fittedPositions);

                fittedPositions = fittedPositions.Take(fittedPositions.Count() - 3).ToArray();
                fittedPressures = fittedPressures.Take(fittedPressures.Count() - 3).ToArray();
                fittedTimes = fittedTimes.Take(fittedTimes.Count() - 3).ToArray();

                if (fittedPositions.Length > 10)
                {
                    //for (int i = 0; i < fittedPositions.Length; i++)
                    //{
                    //    _fittedLineSeries.Points.Add(new DataPoint(
                    //       fittedPositions[i], fittedPressures[i]));
                    //}
                    //DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    //{
                    //    PlotModel.InvalidatePlot(true);
                    //});

                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;
        }
    }
}
