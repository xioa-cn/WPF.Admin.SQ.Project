using CMS.ReaderConfigLIbrary.Models;

namespace CMS.ReaderConfigLIbrary.Utils {
    public static class HeartbeatTypeGetBeatType {
        public static Type GetBeatType(this HeartbeatType heartbeatType) {
            return heartbeatType switch {
                HeartbeatType.Bool => typeof(bool),
                HeartbeatType.Int => typeof(int),
                HeartbeatType.Float => typeof(float),
                HeartbeatType.Double => typeof(double),
                HeartbeatType.ByteArray => typeof(byte[]),
                HeartbeatType.Short => typeof(short),
                HeartbeatType.Long => typeof(long),
                HeartbeatType.Ushort => typeof(ushort),
                HeartbeatType.Ulong => typeof(ulong),
                HeartbeatType.Uint => typeof(uint),
                HeartbeatType.Byte => typeof(byte),
                HeartbeatType.Sbyte => typeof(sbyte),
                HeartbeatType.String => typeof(string),
                _ => typeof(short) 
            };
        }
    }
}