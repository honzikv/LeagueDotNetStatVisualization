using System.Text.Json.Serialization;
using dotNetMVCLeagueApp.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using dotNetMVCLeagueApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dotNetMVCLeagueApp {
    public partial class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public RiotApiUpdateConfig RiotApiUpdateConfig { get; private set; }

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
            services.AddDefaultIdentity<IdentityUser>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<LeagueDbContext>();

            // Pridani automapperu
            services.AddAutoMapper(typeof(Startup));

            // Pridani MVC a nastaveni ReferenceHandler na Preserve pro test controlleru
            services.AddControllersWithViews().AddJsonOptions(config => {
                config.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

            // Konfigurace uzivatelskeho dependency injection pro prehlednost
            ConfigureUserServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}