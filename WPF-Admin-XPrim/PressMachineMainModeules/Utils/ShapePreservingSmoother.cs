using OxyPlot;


namespace PressMachineMainModeules.Utils
{
    public class AdaptiveInterpolationAlgorithm : IInterpolationAlgorithm
    {

        private readonly double alpha;
        private readonly int maxSegments;
        private readonly double curvatureThreshold;

        public AdaptiveInterpolationAlgorithm(double alpha = 0.5, double curvatureThreshold = 0.1)
        {
            this.alpha = alpha;
            this.maxSegments = 100;
            this.curvatureThreshold = curvatureThreshold;
        }

        public List<DataPoint> CreateSpline(List<DataPoint> points, bool isClosed, double tolerance)
        {
            if (points == null || points.Count < 2)
                return points;

            // 1. 首先检测关键点（极值点和曲率变化大的点）
            var keyPoints = DetectKeyPoints(points, isClosed);

            // 2. 根据关键点将曲线分段
            var segments = CreateSegments(points, keyPoints, isClosed);

            // 3. 对每段分别进行插值处理
            var result = new List<DataPoint>();
            foreach (var segment in segments)
            {
                var interpolatedSegment = InterpolateSegment(segment, tolerance);
                result.AddRange(interpolatedSegment);
            }

            return result;
        }

        private List<int> DetectKeyPoints(List<DataPoint> points, bool isClosed)
        {
            var keyPoints = new List<int>();
            int n = points.Count;

            // 检测极值点和曲率变化显著的点
            for (int i = 1; i < n - 1; i++)
            {
                // 检查是否是极值点
                bool isExtremum = IsExtremumPoint(points[i - 1], points[i], points[i + 1]);

                // 检查曲率变化
                bool isHighCurvature = IsCurvatureSignificant(points[i - 1], points[i], points[i + 1]);

                if (isExtremum || isHighCurvature)
                {
                    keyPoints.Add(i);
                }
            }

            // 确保起点和终点被包含
            if (!keyPoints.Contains(0)) keyPoints.Insert(0, 0);
            if (!keyPoints.Contains(n - 1)) keyPoints.Add(n - 1);

            return keyPoints;
        }

        private bool IsExtremumPoint(DataPoint p1, DataPoint p2, DataPoint p3)
        {
            // 检查X方向的极值
            bool isXExtremum = (p2.X > p1.X && p2.X > p3.X) || (p2.X < p1.X && p2.X < p3.X);

            // 检查Y方向的极值
            bool isYExtremum = (p2.Y > p1.Y && p2.Y > p3.Y) || (p2.Y < p1.Y && p2.Y < p3.Y);

            return isXExtremum || isYExtremum;
        }

        private bool IsCurvatureSignificant(DataPoint p1, DataPoint p2, DataPoint p3)
        {
            // 计算两个向量
            var v1 = new Vector(p2.X - p1.X, p2.Y - p1.Y);
            var v2 = new Vector(p3.X - p2.X, p3.Y - p2.Y);

            // 计算向量夹角的余弦值
            double dotProduct = v1.X * v2.X + v1.Y * v2.Y;
            double v1Length = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            double v2Length = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);
            double cosAngle = dotProduct / (v1Length * v2Length);

            // 如果余弦值小于阈值，说明曲率变化显著
            return cosAngle < (1 - curvatureThreshold);
        }

        private List<List<DataPoint>> CreateSegments(List<DataPoint> points, List<int> keyPoints, bool isClosed)
        {
            var segments = new List<List<DataPoint>>();

            // 根据关键点划分段
            for (int i = 0; i < keyPoints.Count - 1; i++)
            {
                var segment = new List<DataPoint>();
                for (int j = keyPoints[i]; j <= keyPoints[i + 1]; j++)
                {
                    segment.Add(points[j]);
                }
                segments.Add(segment);
            }

            // 处理闭合曲线的情况
            if (isClosed && !points[0].Equals(points[points.Count - 1]))
            {
                var lastSegment = new List<DataPoint>();
                for (int i = keyPoints[keyPoints.Count - 1]; i < points.Count; i++)
                {
                    lastSegment.Add(points[i]);
                }
                lastSegment.Add(points[0]);
                segments.Add(lastSegment);
            }

            return segments;
        }

        private List<DataPoint> InterpolateSegment(List<DataPoint> segment, double tolerance)
        {
            var result = new List<DataPoint>();
            int n = segment.Count;

            for (int i = 0; i < n - 1; i++)
            {
                var p0 = i > 0 ? segment[i - 1] : segment[i];
                var p1 = segment[i];
                var p2 = segment[i + 1];
                var p3 = i < n - 2 ? segment[i + 2] : segment[i + 1];

                // 根据点的特征调整插值密度
                int segments = CalculateSegments(p1, p2, tolerance);

                // 使用Catmull-Rom样条进行插值
                for (double t = 0; t <= 1; t += 1.0 / segments)
                {
                    result.Add(CatmullRomPoint(p0, p1, p2, p3, t));
                }
            }

            return result;
        }

        

        private struct Vector
        {
            public double X { get; }
            public double Y { get; }

            public Vector(double x, double y)
            {
                X = x;
                Y = y;
            }
        }



























        public List<ScreenPoint> CreateSpline(IList<ScreenPoint> points, bool isClosed, double tolerance)
        {
            if (points == null || points.Count < 2)
                return points.ToList();

            var result = new List<ScreenPoint>();
            int n = points.Count;

            // 处理闭合曲线
            if (isClosed && !points[0].Equals(points[n - 1]))
            {
                ((List<ScreenPoint>)points).Add(points[0]);
                n++;
            }

            // 对每个线段进行插值
            for (int i = 0; i < n - 1; i++)
            {
                var p0 = i > 0 ? points[i - 1] : points[i];
                var p1 = points[i];
                var p2 = points[i + 1];
                var p3 = i < n - 2 ? points[i + 2] : p2;

                // 计算插值点
                int segments = CalculateSegments(p1, p2, tolerance);
                for (double t = 0; t <= 1; t += 1.0 / segments)
                {
                    result.Add(CatmullRomPoint(p0, p1, p2, p3, t));
                }
            }

            return result;
        }

        private int CalculateSegments(DataPoint p1, DataPoint p2, double tolerance)
        {
            double distance = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            int segments = (int)(distance / tolerance);
            return Math.Min(Math.Max(segments, 2), maxSegments);
        }

        private int CalculateSegments(ScreenPoint p1, ScreenPoint p2, double tolerance)
        {
            double distance = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            int segments = (int)(distance / tolerance);
            return Math.Min(Math.Max(segments, 2), maxSegments);
        }

        private DataPoint CatmullRomPoint(DataPoint p0, DataPoint p1, DataPoint p2, DataPoint p3, double t)
        {
            double t2 = t * t;
            double t3 = t2 * t;

            double x = 0.5 * ((2 * p1.X) +
                (-p0.X + p2.X) * t +
                (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2 +
                (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3);

            double y = 0.5 * ((2 * p1.Y) +
                (-p0.Y + p2.Y) * t +
                (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 +
                (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3);

            return new DataPoint(x, y);
        }

        private ScreenPoint CatmullRomPoint(ScreenPoint p0, ScreenPoint p1, ScreenPoint p2, ScreenPoint p3, double t)
        {
            double t2 = t * t;
            double t3 = t2 * t;

            double x = 0.5 * ((2 * p1.X) +
                (-p0.X + p2.X) * t +
                (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2 +
                (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3);

            double y = 0.5 * ((2 * p1.Y) +
                (-p0.Y + p2.Y) * t +
                (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 +
                (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3);

            return new ScreenPoint(x, y);
        }
    }
}
