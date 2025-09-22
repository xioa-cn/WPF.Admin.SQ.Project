using External.GrpcServices.Utils;
using Grpc.Core;
using GrpcServices;
using WPF.Admin.Service.Services.Grpcs;

namespace External.GrpcServices.Services
{
    [GrpcService]
    public class NormalTestGrpcServerService : NormalTestGrpcServer.NormalTestGrpcServerBase
    {
        [RequiredRole(Permissions.Admin)]
        public override Task<TestResponse> Test(TestRequest request, ServerCallContext context)
        {
            return Task.FromResult(new TestResponse { State = "OK" });
        }
    }
}