using WPF.Admin.Models.Models;

namespace WPF.Admin.Models.Utils {
    public static class AlarmLogHelper {
        public static AlarmLog SystemLog(string systemMessage) {
            if (AppSettings.Default is null || !AppSettings.Default.OpenAlarmLog)
            {
                throw new Exception("OpenAlarmLog is false");
            }

            return new AlarmLog {
                Message = systemMessage,
                Type = AlarmType.System
            };
        }

        public static AlarmLog DeviceLog(string deviceMessage) {
            if (AppSettings.Default is null || !AppSettings.Default.OpenAlarmLog)
            {
                throw new Exception("OpenAlarmLog is false");
            }
            return new AlarmLog() {
                Message = deviceMessage,
                Type = AlarmType.Device
            };
        }
    }
}