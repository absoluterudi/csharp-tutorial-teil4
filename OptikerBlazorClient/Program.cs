using BlazorDownloadFile;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BarBuddy.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddBlazorise(options =>
            {
                options.ChangeTextOnKeyPress = true; // optional
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

            builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            var serverApiUrl = builder.Configuration.GetValue<string>("ServerApiUrl");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverApiUrl) });

            await builder.Build().RunAsync();
        }
    }
}
