using BarBuddy.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BarBuddy.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();
            builder.Services.AddBlazoredLocalStorage();

            var serverApiUrl = builder.Configuration.GetValue<string>("ServerApiUrl");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverApiUrl) });

            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IQRCodeService, QRCodeService>();

            await builder.Build().RunAsync();
        }
    }
}
