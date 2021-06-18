using AutoMapper;
using BarBuddy.Server.Entities;
using BarBuddy.Server.DataContext;
using BarBuddyBackend.Services;
using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BarBuddyBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddControllers();

            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = false; // optional
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            services.AddBlazoredToast();

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(connection, x => x.UseNetTopologySuite()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BarBuddy", Version = "v1" });
            });

            services.AddScoped<EmailService>();
            services.AddScoped<GoogleMapsService>();
            services.AddScoped<HtmlRenderService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<CompanySpotService>();
            services.AddScoped<UserService>();
            services.AddScoped<Session>();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new MappingConfig());
            }).CreateMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BarBuddy v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
