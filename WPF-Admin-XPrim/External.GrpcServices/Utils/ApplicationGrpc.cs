using External.GrpcServices.Start;

namespace External.GrpcServices.Utils {
    public static class ApplicationGrpc {
        public static void AppGrpcServices() {
            Task.Run(StartupGrpc.Startup);
        }
    }
}