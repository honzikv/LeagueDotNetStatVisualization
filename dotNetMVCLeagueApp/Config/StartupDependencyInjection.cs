using System;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Services.MatchHistory;
using dotNetMVCLeagueApp.Services.MatchTimeline;
using Microsoft.Extensions.DependencyInjection;
using MingweiSamuel.Camille;

// ReSharper disable once CheckNamespace
namespace dotNetMVCLeagueApp {
    /// <summary>
    /// Druha cast tridy Startup, zde je prehledneji videt co se bude davat do dependency containeru
    /// </summary>
    public partial class Startup {
        private void ConfigureUserServices(IServiceCollection services) {
            services.AddSingleton(_ => RiotApi.NewInstance(
                apiKey: Configuration["RiotApiKey"]
            ));
            
            // TODO: momentalne je na 0, nasledne bude jeste zmeneno
            services.AddSingleton(_ => new RiotApiUpdateConfig(TimeSpan.FromMinutes(0)));

            // Repozitare
            services.AddScoped<RiotApiRepository>(); // Komunikace s Riot Api
            services.AddScoped<SummonerRepository>();
            services.AddScoped<MatchRepository>();
            services.AddScoped<QueueInfoRepository>();
            services.AddScoped<MatchInfoSummonerInfoRepository>();
            services.AddScoped<MatchTimelineRepository>();
            
            // Services - wrapper nad repozitari, ktery se vola z controlleru
            services.AddScoped<SummonerInfoService>(); // Pro info o hracich
            services.AddScoped<MatchHistoryService>(); // Pro info o zapasech
            services.AddScoped<SummonerProfileStatsService>(); // Pro vypocty statistik
            services.AddScoped<MatchTimelineService>();
        }
    }
}