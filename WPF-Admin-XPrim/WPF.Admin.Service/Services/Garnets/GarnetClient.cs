using StackExchange.Redis;

namespace WPF.Admin.Service.Services.Garnets {
    public class GarnetClient {
        public static async Task<ConnectionMultiplexer> ConnectGarnetAsync(string connect) {
            return await ConnectionMultiplexer.ConnectAsync(connect);
        }
    }
}