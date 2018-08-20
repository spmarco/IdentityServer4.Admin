using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.AspNetIdentity.Util;

namespace Skoruba.IdentityServer4.AspNetIdentity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            // Uncomment this to seed upon startup, alternatively pass in `dotnet run / seed` to seed using CLI
            // await DbMigrationHelpers.EnsureSeedData(host);
            Task.WaitAll(DbMigrationHelpers.EnsureSeedData(host));

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
