using WPF.Admin.Models.Db;

namespace WPF.Admin.Models.Utils {
    public static class AlarmDbInstance {
        private static readonly string AlarmLogPath =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XLogs", "DBLOGGER");

        private static readonly string AlarmLogFile =
            System.IO.Path.Combine(AlarmLogPath, "Alarm.db");

        public static AlarmDbContext CreateNormal() {
            AlarmDbContext db = new AlarmDbContext(AlarmLogFile);
            if (System.IO.File.Exists(AlarmLogFile))
            {
                return db;
            }

            if (!System.IO.Directory.Exists(AlarmLogPath))
            {
                System.IO.Directory.CreateDirectory(AlarmLogPath);
            }

            db.Database.EnsureCreated();

            return db;
        }

        public static void VerificationAlarmDbContext() {
            if (System.IO.File.Exists(AlarmLogFile))
            {
                return;
            }

            using (CreateNormal())
            {
                
            }
        }
    }
}