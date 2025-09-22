using System.Diagnostics;
using OxyPlot;
using PressMachineMainModeules.Utils;

namespace AutoPreTest
{
    public class FindIndex
    {
        [Fact]
        public void Test()
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            Random r = new Random();
            List<float> array = [0];
            foreach (int i1 in Enumerable.Range(1, 1000))
            {
                array.Add(i1 + (float)r.Next(0, 10) / 10);
                var result =
                    OptimizedValueFinder.FindValueStatus(array.ToArray(), r.Next(0, 1000), out var i);
                LinePositionChecker.GetPointPositionRelativeToLine(new DataPoint(r.Next(0, 1000), r.Next(0, 1000)),
                    new DataPoint(r.Next(0, 1000), r.Next(0, 1000)), new DataPoint(r.Next(0, 1000), r.Next(0, 1000)));
            }
            sp.Stop();
            Debug.WriteLine(sp.ElapsedMilliseconds);
        }
    }
}