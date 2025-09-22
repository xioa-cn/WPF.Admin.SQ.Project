namespace PressMachineMainModeules.Utils
{
    // <summary>
    /// 查找结果的状态枚举
    /// </summary>
    public enum FindStatus
    {
        Exists,
        NotExists_InRange,
        NotExists_OutOfRange
    }

    public class OptimizedValueFinder
    {
        // 浮点数比较精度（根据业务场景调整，默认1e-9）
        private const double Epsilon = 1e-9;

        public static FindStatus FindValueStatus(float[] sortedArray, float value, out int index)
        {
            index = -1;

            if (sortedArray == null || sortedArray.Length == 0)
                return FindStatus.NotExists_OutOfRange;

            int len = sortedArray.Length;
            double first = sortedArray[0];
            double last = sortedArray[len - 1];

            // 快速范围检查（直接比较，性能优先）
            if (value < first || value > last)
                return FindStatus.NotExists_OutOfRange;

            // 单元素数组优化
            if (len == 1)
            {
                if (IsEqual(first, value))
                {
                    index = 0;
                    return FindStatus.Exists;
                }

                return FindStatus.NotExists_InRange;
            }

            // 二分查找核心
            int left = 0;
            int right = len - 1;
            while (left <= right)
            {
                int mid = left + ((right - left) >> 1);
                double midVal = sortedArray[mid];

                // 先进行直接比较（快速路径）
                if (midVal == value)
                {
                    index = mid;
                    return FindStatus.Exists;
                }
                // 再进行范围判断（减少高精度比较）
                else if (midVal > value)
                {
                    right = mid - 1;
                }
                else // midVal < value
                {
                    left = mid + 1;
                }
            }

            // 循环结束后，检查插入位置附近是否有近似值（处理精度误差）
            // 仅在插入位置有效时检查
            if (left < len && IsEqual(sortedArray[left], value))
            {
                index = left;
                return FindStatus.Exists;
            }

            if (left > 0 && IsEqual(sortedArray[left - 1], value))
            {
                index = left - 1;
                return FindStatus.Exists;
            }

            index = left;
            return FindStatus.NotExists_InRange;
        }

        /// <summary>
        /// 安全的浮点数比较（处理精度误差）
        /// </summary>
        private static bool IsEqual(double a, double b)
        {
            // 对接近零的数特殊处理，避免绝对值过小导致的误差放大
            if (Math.Abs(a) < Epsilon && Math.Abs(b) < Epsilon)
                return true;

            // 相对误差检查（适用于大多数场景）
            return Math.Abs(a - b) < Epsilon * Math.Max(Math.Abs(a), Math.Abs(b));
        }
    }
}