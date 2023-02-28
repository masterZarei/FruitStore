using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(FS.FruitStore.Areas.Identity.IdentityHostingStartup))]
namespace FS.FruitStore.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}