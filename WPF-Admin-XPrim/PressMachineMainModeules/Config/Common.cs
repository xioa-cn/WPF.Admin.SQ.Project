using PressMachineMainModeules.Models;
using System.Collections.ObjectModel;
using WPF.Admin.Themes.I18n;

namespace PressMachineMainModeules.Config
{
    internal class Common
    {
        public static Func<string, string> t { get; }

        public static ObservableCollection<MonitorRec> MonitorRecs { get; set; }
            = new ObservableCollection<MonitorRec>();

        public static CurvePara CurvePara { get; set; } = new CurvePara();

        public static ObservableCollection<Step> Steps { get; set; }
            = new ObservableCollection<Step>();

        public static bool DetectInflection = false; //开启拐点
        public static bool InflectionPosLimit = false; // 拐点位置限制
        public static float InflectionSlopeLimit = 1; // 拐点斜率限制
        public static int PressureRoundDigits = 5; //斜率满足点数量
        public static int PointSpan = 3; //拐点间隔
        public static int InterpolationSpan = 5;
        public static string? DataFolder => SysConfigModel.DataFolder;

        public static string? Ip => Virtual ? "127.0.0.1" : SysConfigModel.Ip;
        public static string? Ip01 => Virtual ? "127.0.0.1" : SysConfigModel.Ip01;
        public static string? Ip02 => Virtual ? "127.0.0.1" : SysConfigModel.Ip02;
        public static string? Ip03 => Virtual ? "127.0.0.1" : SysConfigModel.Ip03;
        public static string? Com => SysConfigModel.Com;

        public static string? Code { get; set; } = "未扫码";

        public static bool Virtual = false;

        public static int Port { get; set; } = 502;

        public static bool UsingCodeList => SysConfigModel.UsingCodeList;

        public static bool OpenIO { get; set; } = true;

        public static SystemConfigModel SysConfigModel { get; set; }

        static Common()
        {
            (t, _) = CSharpI18n.UseI18n();
            SysConfigModel = SystemConfigModel.Create();
        }
    }
}