using System.Reflection;
using Grpc.Core;

namespace External.GrpcServices.Utils
{
    public class AuthInterceptor : Grpc.Core.Interceptors.Interceptor
    {
        private readonly ILogger<AuthInterceptor> _logger;
        private readonly Dictionary<string, Type> _serviceTypeMap;

        public AuthInterceptor(ILogger<AuthInterceptor> logger)
        {
            _logger = logger;
        
            // 自动扫描服务类型
            _serviceTypeMap = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType != null && 
                            t.BaseType.IsGenericType )
                .ToDictionary(
                    t => t.Name.Replace("Service", ""),
                    t => t,
                    StringComparer.OrdinalIgnoreCase
                );
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request, ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation) where TRequest : class where TResponse : class
        {
            var token = context.RequestHeaders
                .FirstOrDefault(h => h.Key == "authorization")?.Value;

            if (token != "your_valid_token")
            {
                // token 解析权限 对比方法的权限
                context.Status = new Status(StatusCode.Unauthenticated, "无效令牌");
                return default;
            }

            return await continuation(request, context);
        }
    }
}