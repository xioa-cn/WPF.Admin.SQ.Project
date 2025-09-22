using System.Reflection;
using External.GrpcServices.Services;
using Microsoft.AspNetCore.Builder;
using WPF.Admin.Service.Services.Grpcs;

namespace External.GrpcServices.Utils {
    public static class MapGrpcServicesHelper {
        public static WebApplication MapGrpcServices(this WebApplication app) {
            app.MapGrpcService<NormalTestGrpcServerService>();
            return app;
        }

        public static WebApplication MapGrpcServices(this WebApplication app, Assembly assembly) {
            var mapGrpcServiceMethod = typeof(GrpcEndpointRouteBuilderExtensions)
                .GetMethods()
                .First(m => 
                    m.Name == "MapGrpcService" && 
                    m.IsGenericMethodDefinition && 
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(IEndpointRouteBuilder));

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.GetCustomAttribute<GrpcServiceAttribute>() != null)
                {
                    var genericMethod = mapGrpcServiceMethod.MakeGenericMethod(type);
                    try
                    {
                        genericMethod.Invoke(null, new object[] { app });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to map gRPC service {type.FullName}: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
            }

            return app;
        }
    }
}