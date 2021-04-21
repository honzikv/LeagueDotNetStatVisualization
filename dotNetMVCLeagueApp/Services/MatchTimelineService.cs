using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba, ktera slouzi pro ziskani timeline pro dany zapas
    /// </summary>
    public class MatchTimelineService {
        private readonly MatchTimelineRepository matchTimelineRepository;
        private ILogger<MatchTimelineService> logger;

        public MatchTimelineService(MatchTimelineRepository matchTimelineRepository,
            ILogger<MatchTimelineService> logger) {
            this.matchTimelineRepository = matchTimelineRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Synchronizovano pro controller
        /// </summary>
        /// <param name="matchId">Id pro zapas</param>
        /// <returns></returns>
        public MatchTimelineModel GetMatchTimelineFromDbAsync(long matchId) =>
            GetMatchTimelineFromDb(matchId).GetAwaiter().GetResult();

        private async Task<MatchTimelineModel> GetMatchTimelineFromDb(long matchId) =>
            await matchTimelineRepository.Get(matchId);
    }
}