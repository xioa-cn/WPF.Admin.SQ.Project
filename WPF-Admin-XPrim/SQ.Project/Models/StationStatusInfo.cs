using SQ.Project.Core;

namespace SQ.Project.Models
{
    public class StationStatusInfo
    {
        public string Plid { get; set; } = Const.Plid;
        public string Wsid { get; set; }
        public int Status { get; set; }
        public string ErrorCode { get; set; }
        public string Msg { get; set; }
    }
}