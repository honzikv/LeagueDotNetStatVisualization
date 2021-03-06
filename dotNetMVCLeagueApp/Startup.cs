using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;
using Castle.Core.Logging;
using dotNetMVCLeagueApp.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Westwind.AspNetCore.LiveReload;
using ILogger = Castle.Core.Logging.ILogger;


namespace dotNetMVCLeagueApp {
    public partial class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Vytvoreni DbContextu
            services.AddDbContext<LeagueDbContext>(options => {
                options.UseSqlite(Configuration.GetConnectionString("LeagueStatsDbSqliteConnString"));
                options.UseLazyLoadingProxies();
            });
            

            // Puvodni SQL Express server
            // services.AddDbContext<LeagueDbContext>(options => {
            //     options.UseSqlServer(Configuration.GetConnectionString("LeagueStatsDbConnectionString"));
            //     options.UseLazyLoadingProxies();
            // });

            services.AddDatabaseDeveloperPageExceptionFilter();

            // services.AddDefaultIdentity<ApplicationUser>(
            //         options => options.SignIn.RequireConfirmedAccount = true)
            //     .AddEntityFrameworkStores<LeagueDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<LeagueDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            
            // Nastaveni restrikci, protoze Microsoft veri tomu, ze nekdo bude realne vyzadovat po uzivateli
            // znak ktery neni alfanumericky v heslu
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            });

            // Pridani automapperu
            services.AddAutoMapper(typeof(Startup));

            // Nastaveni rootu pro json soubory s konfiguraci assetu
            Configuration["Assets:JsonRoot"] =
                Path.Combine(Configuration.GetValue<string>(WebHostDefaults.ContentRootKey), "AssetJson");

            // Pridani MVC a nastaveni ReferenceHandler na Preserve pro test controlleru
            services.AddControllersWithViews().AddJsonOptions(config => {
                config.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
            services.AddRazorPages();
            
            // Konfigurace uzivatelskeho dependency injection pro prehlednost
            ConfigureUserServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Protoze tato sluzba cte json je rozumne ji vytvorit primo pri startupu aby se pri prvnim
            // requestu, ktery ji potrebuje zbytecne necekalo
            app.ApplicationServices.GetService<AssetRepository>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Identity
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                // // Deaktivovano, protoze aplikace controllery nakonec nepouziva
                // endpoints.MapControllerRoute(
                //     "default",
                //     "{controller=Home}/{action=Index}/{id?}"
                // );
                endpoints.MapRazorPages();
            });
            
        }
    }
}