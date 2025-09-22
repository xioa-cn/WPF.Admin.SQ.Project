
namespace PressMachineMainModeules.Utils
{
    public class LineInterpolation
    {
        public static double[] Interpolate(double[] rawXs, double[] rawYs, double[] targetXs)
        {
            if (rawXs == null
                || rawYs == null
                || targetXs == null
                || rawXs.Length < 10
                || rawYs.Length < 10
                || targetXs.Length < 10)
            {
                throw new Exception("Argument cannot be null and elements of array are not enough.");
            }

            // Remove the values that are same, just preserve one.
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            int count = rawXs.Length;
            double temp = rawXs[0];
            xs.Add(rawXs[0]);
            ys.Add(rawYs[0]);
            for (int i = 1; i < count; i++)
            {
                if (rawXs[i] > temp)
                {
                    xs.Add(rawXs[i]);
                    ys.Add(rawYs[i]);
                    temp = rawXs[i];
                }
            }

            List<double> rets = new List<double>();

            if (xs.Count < 2)
            {
                return rets.ToArray();
            }


            int start = 0;
            for (int i = 0; i < targetXs.Length; i++)
            {
                double ret;
                Tuple<double, double> para;
                (double, double) para1;
                if (targetXs[i] < xs[0])
                {
                    para1 = MathNet.Numerics.Fit.Line(
                                new double[] { xs[0], xs[1] },
                                new double[] { ys[0], ys[1] });
                    para = new Tuple<double, double>(para1.Item1, para1.Item2);
                    ret = para.Item1 + targetXs[i] * para.Item2;
                    rets.Add(ret);
                }
                else if (targetXs[i] > xs[xs.Count - 1])
                {
                    para1 = MathNet.Numerics.Fit.Line(
                               new double[] { xs[xs.Count - 2], xs[xs.Count - 1] },
                               new double[] { ys[ys.Count - 2], ys[ys.Count - 1] });
                    para = new Tuple<double, double>(para1.Item1, para1.Item2);
                    ret = para.Item1 + targetXs[i] * para.Item2;
                    rets.Add(ret);
                }
                else
                {
                    for (int j = start; j < xs.Count; j++)
                    {
                        if (targetXs[i] == xs[j])
                        {
                            rets.Add(ys[j]);
                            break;
                        }
                        else if (targetXs[i] < xs[j])
                        {
                            //double slope = (ys[j]- ys[j-1]) / (xs[j] - xs[j - 1]);
                            //double intercept=
                            para1 = MathNet.Numerics.Fit.Line(
                               new double[] { xs[j - 1], xs[j] },
                               new double[] { ys[j - 1], ys[j] });
                            para = new Tuple<double, double>(para1.Item1, para1.Item2);
                            ret = para.Item1 + targetXs[i] * para.Item2;
                            rets.Add(ret);
                            break;
                        }
                        else
                        {
                            start = j;
                            continue;
                        }
                    }
                }
            }
            return rets.ToArray();
        }
    }
}
