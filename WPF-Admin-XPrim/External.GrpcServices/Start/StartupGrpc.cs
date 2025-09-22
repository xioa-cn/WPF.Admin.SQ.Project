using System.Net.Http;
using System.Reflection;
using External.GrpcServices.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;

namespace External.GrpcServices.Start {
    public class StartupGrpc {
        public static void Startup() {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                //options.Interceptors.Add<AuthInterceptor>(); // 权限拦截器
            });
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureEndpointDefaults(endpointOptions =>
                {
                    endpointOptions.Protocols = HttpProtocols.Http2;
                });
            });

            var app = builder.Build();

            // app.MapGrpcServices();
            app.MapGrpcServices(typeof(StartupGrpc).Assembly);
            app.MapGet("/", () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}