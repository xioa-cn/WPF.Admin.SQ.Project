using CMS.ReaderConfigLIbrary.Services;

namespace CMS.Plcs.Test {
    public class ConnectServicesTest {
        [Fact]
        public void Test1() {
            var ret = ConnectServices.PlcsConnect();
        }
    }
}