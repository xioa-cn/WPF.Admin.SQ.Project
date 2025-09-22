using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WPF.Admin.Models.Background;

namespace WPFAdmin
{
    public partial class App
    {
        private IHost? _host;

        private void UsingHostServices()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<QueuedHostedService>();
                })
                .Build();
            
            _host.Start();
        }
    }
}