using System.Diagnostics;

namespace PressMachineMainModeules.Utils
{
    public class InflectionFinder
    {
        public static int Find(double[] xs, double[] ys, float? slopeLimit = null, int? roundDigits = null)
        {
            if (roundDigits.HasValue)
            {
                for (int i = 0; i < ys.Length; i++)
                {
                    //var temp = ys[i];
                    var factor = Math.Pow(10, roundDigits.Value);
                    xs[i] = Math.Floor(xs[i] * factor) / factor;
                    ys[i] = Math.Floor(ys[i] * factor) / factor;
                    //Debug.WriteLine($"{temp}  ||  {ys[i]}");
                }
            }

            double lastSlope = 0;
            double maxSlopeDiffer = double.MinValue;
            int index = 0;

            for (int i = 0; i < xs.Length; i++)
            {
                if (i > 0)
                {
                    double x1 = xs[i - 1];
                    double x2 = xs[i];
                    double y1 = ys[i - 1];
                    double y2 = ys[i];

                    var para = MathNet.Numerics.Fit.Line(
                               new double[] { x2, x1 },
                               new double[] { y2, y1 });
                    double slope = para.Item2;
                    if (xs[i] == xs[i - 1])
                    {
                        slope = 0;
                    }

                    if (i > 1)
                    {
                        var slopeDiffer = slope - lastSlope;

                        if (y2 - y1 > 0.02 && slope > 0 && slopeDiffer > maxSlopeDiffer)
                        {
                            maxSlopeDiffer = slopeDiffer;
                            index = i;
                        }

                    }
                    lastSlope = slope;

                    if (slopeLimit.HasValue && slope > slopeLimit)
                    {
                        Debug.WriteLine($"截止索引：{i}; Pos: {xs[i]}; Pre: {ys[i]}");
                        break;
                    }
                }
            }

            return index == 0 ? 0 : index - 1;
        }
    }
}
