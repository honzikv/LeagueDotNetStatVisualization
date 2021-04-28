using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner {
    public class SummonerOverviewDto {

        public SummonerModel Summoner;
        
        /// <summary>
        /// Statistiky pro seznam s MatchHeaders
        /// </summary>
        public MatchListStatsDto MatchListStatsDto { get; }

        /// <summary>
        /// Seznam zapasu serazeny podle datumu
        /// </summary>
        public List<MatchHeaderDto> MatchHeaders { get; }

        public SummonerOverviewDto(SummonerModel summoner, MatchListStatsDto matchListStats, List<MatchHeaderDto> matchHeaders) {
            Summoner = summoner;
            MatchListStatsDto = matchListStats;
            MatchHeaders = matchHeaders;
        }
    }
}