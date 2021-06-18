using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BarBuddyBackend.Seed
{
    public static class DataSeeder
    {
        public static void SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDBContext>();

                context.Database.EnsureCreated();

                if (!context.Bars.Any())
                {
                    var path = Directory.GetCurrentDirectory() + "//Seed//create_init.sql";
                    var script = File.ReadAllText(path);
                    context.Database.ExecuteSqlRaw(script, new List<object>());
                }
            }
        }
    }
}
