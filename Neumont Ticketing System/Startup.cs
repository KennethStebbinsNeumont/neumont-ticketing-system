using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Neumont_Ticketing_System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neumont_Ticketing_System.Models;
using Microsoft.Extensions.Options;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Areas.Identity.Data;

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

            // Adding Identity database
            services.Configure<IdentityDatabaseSettings>(
                Configuration.GetSection(nameof(IdentityDatabaseSettings)));
            services.AddSingleton<IIdentityDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<IdentityDatabaseSettings>>().Value);
            services.AddSingleton<AppIdentityStorageService>();
            services.AddTransient<IUserStore<AppUser>, AppUserStore>();
            services.AddTransient<IRoleStore<AppRole>, AppRoleStore>();

            // Adding custom identity service
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identitybuilder?view=aspnetcore-3.1
            services.AddIdentity<AppUser, AppRole>()
                .AddRoleStore<AppRoleStore>()
                .AddUserStore<AppUserStore>()
                .AddRoleManager<AppRoleManager>()
                .AddUserManager<AppUserManager>()
                .AddSignInManager<AppSignInManager>()
                .AddDefaultTokenProviders();

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
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
            app.UseHttpsRedirection();
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
