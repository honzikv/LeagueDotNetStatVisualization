using System;
using dotNetMVCLeagueApp.Config;
using LeagueStatAppReact.Repositories;
using LeagueStatAppReact.Services;
using Microsoft.Extensions.DependencyInjection;
using MingweiSamuel.Camille;

// ReSharper disable once CheckNamespace
namespace LeagueStatAppReact {
    public partial class Startup {
        private void ConfigureUserServices(IServiceCollection services) {
            services.AddSingleton(_ => RiotApi.NewInstance(
                apiKey: Configuration["RiotApiKey"]
            ));

            // User can update once per minute (can be changed in the appsettings.json config file)
            services.AddSingleton(_ => new RiotApiUpdateConfig(
                        TimeSpan.FromMinutes(Convert.ToInt32(Configuration["SummonerUpdateMinutes"]))));

            // Add repositories
            services.AddScoped<RiotApiRepository>();
            services.AddScoped<SummonerInfoRepository>();
            services.AddScoped<MatchInfoRepository>();
            
            // Add services
            services.AddScoped<SummonerInfoService>();
            services.AddScoped<MatchHistoryService>();
        }
    }
}