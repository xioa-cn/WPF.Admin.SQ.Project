using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils
{
    public static class PointHelper
    {
        public static int FindClosestPointIndex(IEnumerable<PosintModel> points, System.Windows.Point target)
        {
            if (points == null || points.Count() == 0)
                return -1;

            if (target == null)
                return -1;

            int closestIndex = -1;
            double minDistanceSquared = double.MaxValue;
            int currentIndex = 0;

            foreach (var point in points)
            {
                // 计算两点之间距离的平方（避免开方运算，提高性能）
                double distanceSquared = Math.Pow(point.X1 - target.X, 2) + Math.Pow(point.Y1 - target.Y, 2);

                // 更新最小距离和索引
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closestIndex = currentIndex;
                }

                currentIndex++;
            }

            return closestIndex;
        }
    }
}
