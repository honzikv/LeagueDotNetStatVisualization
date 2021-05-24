using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba v pozadi, ktera maze stare hry
    /// </summary>
    public class MatchDeleteBackgroundService : IHostedService, IDisposable {
        /// <summary>
        /// Pro ziskani zavislosti
        /// </summary>
        private readonly IServiceScopeFactory scopeFactory;
        
        private readonly ILogger<MatchDeleteBackgroundService> logger;
        
        /// <summary>
        /// Doba opakovani projizdeni databaze a mazani zaznamu
        /// </summary>
        private readonly TimeSpan period;
        
        /// <summary>
        /// Config objekt s informacemi o tom, co se ma mazat
        /// </summary>
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;
        
        /// <summary>
        /// Vlakno pro mazani
        /// </summary>
        private Timer timer;

        public MatchDeleteBackgroundService(IServiceScopeFactory scopeFactory,
            ILogger<MatchDeleteBackgroundService> logger,
            RiotApiUpdateConfig riotApiUpdateConfig,
            TimeSpan period) {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.period = period;

            logger.LogDebug("MatchDeleteBackground service instantiated");
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            // Pri startu sluzby vytvorime timer - vlakno, ktere bude podle nejake periody kontrolovat db a 
            // smaze irrelevantni zapasy
            timer = new Timer(Work, null, TimeSpan.Zero, period);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Funkce, ktera se periodicky provadi
        /// </summary>
        /// <param name="state"></param>
        private void Work(object state) {
            logger.LogInformation("MatchDeleteBackground service is starting work");
            using var scope = scopeFactory.CreateScope();
            try {
                // Ziskame novy repozitar pro zapasy
                var matchRepository = new MatchRepository(scope.ServiceProvider.GetRequiredService<LeagueDbContext>());
                var deletedGames =
                    matchRepository.DeleteOldMatches(DateTime.Now.Subtract(riotApiUpdateConfig.MaxMatchAgeDays))
                    .GetAwaiter().GetResult();

                if (deletedGames is null) {
                    return;
                }

                if (deletedGames.Count > 0) {
                    var games = string.Join(';',
                        deletedGames.Select(game => $"id - {game.Id}, playTime - {game.PlayTime}"));
                    logger.LogInformation($"Deleted old games ({deletedGames.Count}). Games: {games}");
                }
                else {
                    logger.LogInformation("No old games were found.");
                }

            }
            catch (InvalidOperationException) {
                logger.LogCritical("Background service (MatchDeleteBackgroundService) could not access database");
            }
            catch (ActionNotSuccessfulException ex) {
                logger.LogCritical(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            logger.LogInformation("Stopping MatchDeleteBackgroundService");
            return Task.CompletedTask;
        }

        public void Dispose() {
            timer?.Dispose();
        }
    }
}