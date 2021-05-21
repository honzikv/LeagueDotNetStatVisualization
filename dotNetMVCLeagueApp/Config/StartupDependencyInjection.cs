using System;
using System.IO;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;

// ReSharper disable once CheckNamespace
namespace dotNetMVCLeagueApp {
    /// <summary>
    /// Druha cast tridy Startup, zde je prehledneji videt co se bude davat do dependency containeru
    /// </summary>
    public partial class Startup {
        private string GetAssetJsonPath(string assetJsonFileName) =>
            Path.Combine(Configuration["Assets:JsonRoot"], assetJsonFileName);

        private void ConfigureUserServices(IServiceCollection services) {
            services.AddSingleton(_ => RiotApi.NewInstance(
                apiKey: Configuration["RiotApiKey"]
            ));

            // Konfigurace pro nacitani assetu
            services.AddSingleton(_ => new AssetLocationConfig {
                ChampionsFolderName = Configuration["Assets:Champions"],
                ItemsFolderName = Configuration["Assets:Items"],
                EmptyAssetFileName = Configuration["Assets:EmptyAsset"],
                ProfileIconsFolderName = Configuration["Assets:ProfileIcons"],
                SummonerSpellsFolderName = Configuration["Assets:SummonerSpells"],
                RankedIconsFolderName = Configuration["Assets:RankedIcons"]
            });

            services.AddSingleton(_ => new RiotApiUpdateConfig(
                TimeSpan.FromMinutes(1),
                TimeSpan.FromDays(30))
            );

            // Repozitare
            services.AddScoped<RiotApiRepository>(); // Komunikace s Riot Api
            services.AddScoped<SummonerRepository>();
            services.AddScoped<MatchRepository>();
            services.AddScoped<QueueInfoRepository>();
            services.AddScoped<MatchInfoSummonerInfoRepository>();
            services.AddScoped<MatchTimelineRepository>();
            services.AddScoped<ApplicationUserRepository>();
            services.AddScoped<ProfileCardRepository>();

            // Asset resolver musi byt jako singleton, protoze nechceme vytvaret objekt pro kazdy request
            services.AddSingleton(serviceProvider => new AssetRepository(
                serviceProvider.GetRequiredService<AssetLocationConfig>(),
                GetAssetJsonPath(Configuration["Assets:ChampionsJson"]),
                GetAssetJsonPath(Configuration["Assets:SummonerSpellsJson"]),
                GetAssetJsonPath(Configuration["Assets:RunesJson"]),
                GetAssetJsonPath(Configuration["Assets:ItemsJson"]),
                GetAssetJsonPath(Configuration["Assets:RankedIconsJson"])
            ));

            // Services - wrapper nad repozitari, ktery se vola z controlleru
            services.AddScoped<SummonerService>(); // Pro info o hracich
            services.AddScoped<SummonerProfileStatsService>(); // Pro vypocty statistik
            services.AddScoped<MatchStatsService>(); // Pro statistiky pro danou hru
            services.AddScoped<MatchService>();
            services.AddScoped<ProfileCardService>();
            services.AddScoped<ApplicationUserService>();
            services.AddScoped<MatchTimelineStatsService>();

            // Sluzba na pozadi, ktera kazdych N (3) minut zkontroluje DB a smaze hry, ktere uz nelze zobrazit
            // - hry, ktere jsou starsi nez M (30) dni.
            services.AddHostedService(serviceProvider =>
                new MatchDeleteBackgroundService(
                    serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                    serviceProvider.GetRequiredService<ILogger<MatchDeleteBackgroundService>>(),
                    serviceProvider.GetService<RiotApiUpdateConfig>(),
                    TimeSpan.FromMinutes(int.Parse(Configuration["OldGamesCheckFrequencyMinutes"]))
                ));
        }
    }
}