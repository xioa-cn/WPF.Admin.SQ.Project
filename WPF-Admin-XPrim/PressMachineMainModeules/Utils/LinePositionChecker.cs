using OxyPlot;

namespace PressMachineMainModeules.Utils
{
    /// <summary>
    /// 点相对于直线的位置
    /// </summary>
    public enum PointPosition
    {
        None,
        /// <summary>
        /// 点在直线上方
        /// </summary>
        Above,

        /// <summary>
        /// 点在直线下方
        /// </summary>
        Below,

        /// <summary>
        /// 点在直线上
        /// </summary>
        OnLine
    }

    public static class LinePositionChecker
    {
        /// <summary>
        /// 判断点相对于由两点确定的直线的位置（上方/下方/线上）
        /// </summary>
        /// <param name="a">直线上的第一个点</param>
        /// <param name="b">直线上的第二个点</param>
        /// <param name="p">要判断的点</param>
        /// <returns>点的相对位置</returns>
        public static PointPosition GetPointPositionRelativeToLine(DataPoint a, DataPoint b, DataPoint p)
        {           
            if(p.X<a.X || p.X > b.X)
            {
                return PointPosition.None;
            }
            double crossProduct = (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);

            // 处理浮点数精度问题
            const double epsilon = 1e-9;
            if (Math.Abs(crossProduct) < epsilon)
            {
                return PointPosition.OnLine;
            }
          
            return crossProduct > 0 ? PointPosition.Above : PointPosition.Below;
        }

       
    }
}