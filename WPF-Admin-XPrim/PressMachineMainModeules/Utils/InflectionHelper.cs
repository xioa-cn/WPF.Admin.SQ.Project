using PressMachineMainModeules.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Utils
{
    public static class InflectionHelper
    {
        public static bool FindInflection(
           double[] fittedPositions,
           double[] fittedPressures, out double inflectionPos, out double infectionPre, CurvePara CurvePara)
        {
            inflectionPos = -9999;
            infectionPre = -9999;

            List<double> xsl = new List<double>();
            List<double> ysl = new List<double>();
            for (int i = 0; i < fittedPositions.Length; i++)
            {
                if (Config.Common.PointSpan > 0)
                {
                    if (Config.Common.InflectionPosLimit)
                    {
                        if (i % Config.Common.PointSpan == 0)
                        {
                            xsl.Add(fittedPositions[i]);
                            ysl.Add(fittedPressures[i]);
                        }
                    }
                    else
                    {
                        if (fittedPositions[i] >= CurvePara.InflexionMinX &&
                            fittedPositions[i] <= CurvePara.InflexionMaxX &&
                            i % Config.Common.PointSpan == 0)
                        {
                            xsl.Add(fittedPositions[i]);
                            ysl.Add(fittedPressures[i]);
                        }
                    }

                }
                else
                {
                    if (Config.Common.InflectionPosLimit)
                    {
                        xsl.Add(fittedPositions[i]);
                        ysl.Add(fittedPressures[i]);
                    }
                    else
                    {
                        if (fittedPositions[i] >= CurvePara.InflexionMinX &&
                            fittedPositions[i] <= CurvePara.InflexionMaxX)
                        {
                            xsl.Add(fittedPositions[i]);
                            ysl.Add(fittedPressures[i]);
                        }
                    }

                }
            }
            var xs = xsl.ToArray();
            var ys = ysl.ToArray();

            if (xs.Length < 1)
                return false;

            int index;
            if (Config.Common.InflectionPosLimit)
            {
                index = InflectionFinder.Find(xs, ys,
                    Config.Common.InflectionSlopeLimit, Config.Common.PressureRoundDigits);
            }
            else
            {
                index = InflectionFinder.Find(xs, ys);
            }
            var targetIndex = index - (Config.Common.PointSpan % 2 == 0 ?
                Config.Common.PointSpan / 2 :
                (Config.Common.PointSpan + 1) / 2);

            if (targetIndex > 0 && targetIndex < xs.Length)
            {
                inflectionPos = xs[targetIndex];
                infectionPre = ys[targetIndex];
                return true;
            }

            return false;
        }
    }
}
