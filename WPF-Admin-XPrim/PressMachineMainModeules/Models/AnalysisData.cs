using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    public class AnalysisData
    {
        public string FileFullName { get; set; }

        public List<MonitorRec> Recs { get; set; }

        public CurvePara CurvePara { get; set; }

        public double[] FittedPostions { get; set; }

        public double[] FittedPressures { get; set; }

        public float InflectionPos { get; set; }

        public float InflectionPre { get; set; }

        public string ProductName { get; set; }

        public string Code { get; set; }

        public Result Ret { get; set; }
    }
}
