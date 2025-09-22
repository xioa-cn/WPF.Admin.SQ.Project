namespace CMS.Plcs.Test {
    public class LoadTest {
        [Fact]
        public void Test1() {
            var temp = ReaderConfigLIbrary.NormalPlcs.Instance["PLC1"];
            foreach (var localDeviceCommunication in ReaderConfigLIbrary.NormalPlcs.DeviceCommunications)
            {
                Console.WriteLine(localDeviceCommunication.Value);
            }
        }
    }
}