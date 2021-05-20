using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba pro vypocet statistik z timeline
    /// </summary>
    public class MatchTimelineStatsService {
        private readonly MatchTimelineRepository matchTimelineRepository;

        private readonly ILogger<MatchTimelineStatsService> logger;

        public MatchTimelineStatsService(MatchTimelineRepository matchTimelineRepository,
            ILogger<MatchTimelineStatsService> logger) {
            this.matchTimelineRepository = matchTimelineRepository;
            this.logger = logger;
        }

    }
}