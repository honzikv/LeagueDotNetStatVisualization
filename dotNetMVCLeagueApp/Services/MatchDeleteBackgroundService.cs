using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba v pozadi, ktera maze hry starsi jeden mesic
    /// </summary>
    public class MatchDeleteBackgroundService : IHostedService, IDisposable {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<MatchDeleteBackgroundService> logger;
        private readonly TimeSpan frequency;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;
        private Timer timer;

        public MatchDeleteBackgroundService(IServiceScopeFactory scopeFactory,
            ILogger<MatchDeleteBackgroundService> logger,
            RiotApiUpdateConfig riotApiUpdateConfig,
            TimeSpan frequency) {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.frequency = frequency;
            
            logger.LogDebug("MatchDeleteBackground service instantiated");
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            timer = new Timer(Work, null, TimeSpan.Zero, frequency);
            return Task.CompletedTask;
        }

        private void Work(object state) {
            logger.LogInformation("MatchDeleteBackground service is starting work");
            using var scope = scopeFactory.CreateScope();
            try {
                var matchRepository = new MatchRepository(scope.ServiceProvider.GetRequiredService<LeagueDbContext>());
                var deletedGames =
                    matchRepository.DeleteOldMatches(DateTime.Now.Subtract(riotApiUpdateConfig.MaxMatchAgeDays))
                        .GetAwaiter().GetResult();

                if (deletedGames is not null) {
                    var games = string.Join(';',
                        deletedGames.Select(game => $"id - {game.Id}, playTime - {game.PlayTime}"));
                    logger.LogInformation($"Deleted old games ({deletedGames.Count}). Games: {games}");
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