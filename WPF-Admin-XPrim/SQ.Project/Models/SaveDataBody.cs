using SQ.Project.Core;

namespace SQ.Project.Models
{
    public class SaveDataBody
    {
        public string Plid { get; set; } = Const.Plid;
        public string Wsid { get; set; }
        public int Result { get; set; }
        public string Itm { get; set; }
        public string KeyMaterial { get; set; }
        public StaticData StaticData { get; set; }
        public string LaserCode { get; set; }
        public string Data { get; set; }
        public Packing Packing { get; set; }
        public string PopOnline { get; set; }
    }

    public class StaticData
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class Packing
    {
        public string PackingCode { get; set; }
        public int Tier { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}