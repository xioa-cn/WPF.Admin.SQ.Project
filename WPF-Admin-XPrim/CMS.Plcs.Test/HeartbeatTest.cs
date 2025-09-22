using CMS.ReaderConfigLIbrary;

namespace CMS.Plcs.Test {
    public class HeartbeatTest {
        [Fact]
        public void BeatTest() {
            var ret =
                NormalPlcs.Instance.ConnectServer();
            if (ret.Result)
            {
                NormalPlcs.Instance.Heartbeat();
            }
        }
    }
}