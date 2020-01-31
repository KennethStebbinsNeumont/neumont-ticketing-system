using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Neumont_Ticketing_System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neumont_Ticketing_System.Models;
using Microsoft.Extensions.Options;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Neumont_Ticketing_System.Models.DatabaseSettings;

namespace Neumont_Ticketing_System
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding MongoDB HelloWorld test
            services.Configure<HelloWorldDatabaseSettings>(
                Configuration.GetSection(nameof(HelloWorldDatabaseSettings)));
            services.AddSingleton<IHelloWorldDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<HelloWorldDatabaseSettings>>().Value);
            services.AddSingleton<HelloWorldService>();

            // Adding Assets database
            services.Configure<AssetsDatabaseSettings>(
                Configuration.GetSection(nameof(AssetsDatabaseSettings)));
            services.AddSingleton<IAssetsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<AssetsDatabaseSettings>>().Value);
            services.AddSingleton<AssetsDatabaseService>();

            // Adding Owners database
            services.Configure<OwnersDatabaseSettings>(
                Configuration.GetSection(nameof(OwnersDatabaseSettings)));
            services.AddSingleton<IOwnersDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<OwnersDatabaseSettings>>().Value);
            services.AddSingleton<OwnersDatabaseService>();

            // Adding Identity database
            services.Configure<IdentityDatabaseSettings>(
                Configuration.GetSection(nameof(IdentityDatabaseSettings)));
            services.AddSingleton<IIdentityDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<IdentityDatabaseSettings>>().Value);
            services.AddSingleton<AppIdentityStorageService>();

            // Adding Tickets database
            services.Configure<TicketsDatabaseSettings>(
                Configuration.GetSection(nameof(TicketsDatabaseSettings)));
            services.AddSingleton<ITicketsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<TicketsDatabaseSettings>>().Value);
            services.AddSingleton<TicketsDatabaseService>();

            // Adding Settings database
            services.Configure<SettingsDatabaseSettings>(
                Configuration.GetSection(nameof(SettingsDatabaseSettings)));
            services.AddSingleton<ISettingsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<SettingsDatabaseSettings>>().Value);
            services.AddSingleton<SettingsDatabaseService>();

            services.AddDbContext<AppIdentityDbContext>();

            // Adding custom identity service
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identitybuilder?view=aspnetcore-3.1
            services.AddIdentity<AppUser, AppRole>()
                .AddRoleStore<AppRoleStore>()
                .AddUserStore<AppUserStore>()
                .AddRoleManager<AppRoleManager>()
                .AddUserManager<AppUserManager>()
                .AddSignInManager<AppSignInManager>()
                .AddDefaultTokenProviders();

            services.Configure<EmailSettings>(Configuration.GetSection(nameof(EmailSettings)));
            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            // For Identity
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
