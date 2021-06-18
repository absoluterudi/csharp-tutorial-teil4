using BarBuddy.Server.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BarBuddy.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            DataSeeder.SeedData(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
