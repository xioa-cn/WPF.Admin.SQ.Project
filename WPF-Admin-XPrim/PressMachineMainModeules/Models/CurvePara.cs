using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    /// <summary>
    /// 坐标参数
    /// </summary>
    [Serializable]
    public class CurvePara
    {
        [Column("MINX")] public float MinX { get; set; } = 0;
        public float MaxX { get; set; } = 200;
        public float MinY { get; set; } = 0;
        public float MaxY { get; set; } = 150;

        public float InflexionMinX { get; set; }
        public float InflexionMaxX { get; set; }
        public float InflexionMinY { get; set; }
        public float InflexionMaxY { get; set; }
    }
}
