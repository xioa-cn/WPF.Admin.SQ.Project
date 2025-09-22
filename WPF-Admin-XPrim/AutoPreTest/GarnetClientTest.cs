using StackExchange.Redis;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Services.Garnets;

namespace AutoPreTest {
    public class GarnetClientTest {
        [Fact]
        public async void Test() {
            var connectString = ApplicationConfigConst.GarnetConnectString;
            var client = await GarnetClient.ConnectGarnetAsync(connectString);
            var db = client.GetDatabase(1);
            db.StringSet("Key", "Value");
            db.HashSet("hash", [new HashEntry("key1", "value1")]);
            var value = db.HashGetAll("hash")[0].Name;
        }

        [Fact]
        public async void Test2() {
            var connectString = ApplicationConfigConst.GarnetConnectString;
            var client = await GarnetClient.ConnectGarnetAsync(connectString);
            var pubsub = client.GetSubscriber();
            pubsub.Publish(RedisChannel.Literal("channel1"), "hello world");
        }

        [Fact]
        public async void Test3() {
            var model = AutoMesPropertiesTest.TestModel();
            var connectString = ApplicationConfigConst.GarnetConnectString;
            var client = await GarnetClient.ConnectGarnetAsync(connectString);
            var db = client.GetDatabase(1);
            db.StringSet("Key", System.Text.Json.JsonSerializer.Serialize(model));
        }
    }
}