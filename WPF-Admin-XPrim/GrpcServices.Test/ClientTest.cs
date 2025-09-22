using Grpc.Net.Client;
using Xunit;

namespace GrpcServices.Test {
    public class ClientTest {
        [Fact]
        public void Test() {
            string url = "http://localhost:5000";
            using (var channel = GrpcChannel.ForAddress(url))
            {
                var client = new GrpcServices.NormalTestGrpcServer.NormalTestGrpcServerClient(channel);
    
                var reply = client.Test(new GrpcServices.TestRequest { Request = "TEST"});
                Console.WriteLine("Greeting: " + reply.State);
            }
        }
    }
}