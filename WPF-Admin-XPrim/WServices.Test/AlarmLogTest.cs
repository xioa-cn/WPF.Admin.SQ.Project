using WPF.Admin.Models.Utils;

namespace WServices.Test {
    public class AlarmLogTest {
        [Fact]
        public void Test() {
            var log =
                AlarmLogHelper.SystemLog("SYSTEM ALARM");
            using (var db = AlarmDbInstance.CreateNormal())
            {
                db.AlarmLogs.Add(log);
                var ret = db.SaveChanges();
                Assert.True(ret > 0);
            }
        }
    }
}