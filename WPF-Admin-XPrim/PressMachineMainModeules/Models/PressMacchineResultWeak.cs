
namespace PressMachineMainModeules.Models
{
    public enum PressMachineResultWeakEnum
    {

        None = 0,
        Clear = 1,
        Normal = 2,
    }

    public class PressMacchineResultWeak
    {
        public string? PressMachineName { get; set; }
        /// <summary>
        /// 对应电缸号
        /// </summary>
        public string? Token { get; set; }
        /// <summary>
        /// 传出结果
        /// </summary>
        public string? Result { get; set; }
        /// <summary>
        /// 拐点位移
        /// </summary>
        public float? InflectionPos { get; set; }
        /// <summary>
        /// 拐点压力
        /// </summary>
        public float? InflectionPre { get; set; }

        public PressMachineResultWeakEnum PressMachineResultWeakEnum { get; set; } = PressMachineResultWeakEnum.None;


        public static PressMacchineResultWeak CreateClearWeak(string? Token)
        {
            return new PressMacchineResultWeak
            {
                Token = Token,
                PressMachineResultWeakEnum = PressMachineResultWeakEnum.Clear,
            };
        }
    }
}
