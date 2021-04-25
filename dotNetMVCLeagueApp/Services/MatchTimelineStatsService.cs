using System.Threading.Tasks;
using Castle.Core.Logging;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.ViewModels.MatchDetail;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Services.Utils;
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

        public MatchTimelineStatsViewModel GetMatchTimelineStatsAsync(MatchInfoModel match) {
            return
        }

        private async Task<MatchTimelineStatsViewModel> GetMatchTimelineStats(MatchInfoModel match) {
            var matchStats = await matchTimelineRepository.Get(match.Id);

            if (matchStats is null) {
                return null;
            }

            // Nicmene pro mody, ktere pouzivame by toto melo byt vzdy 10,
            // pro jine mody muze byt i 5 nebo 8 ucastniku ...
            var numOfPlayers = match.PlayerInfoList.Count;
            var result = new MatchTimelineStatsViewModel(numOfPlayers, matchStats.FrameInterval);

            foreach (var matchTimeFrame in matchStats.MatchFrames) {
                TimelineStatsUtils.AddMatchTimeFrame(result, matchTimeFrame);
            }
        }
    }
}