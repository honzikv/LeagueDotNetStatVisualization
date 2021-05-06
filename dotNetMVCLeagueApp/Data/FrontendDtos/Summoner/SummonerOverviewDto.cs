using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner {
    public class SummonerOverviewDto {

        public SummonerModel Summoner { get; }
        
        /// <summary>
        /// Statistiky pro seznam s MatchHeaders
        /// </summary>
        public MatchListOverviewDto MatchListOverviewDto { get; }

        /// <summary>
        /// Seznam zapasu serazeny podle datumu
        /// </summary>
        public List<MatchHeaderDto> MatchHeaders { get; }

        public SummonerOverviewDto(SummonerModel summoner, MatchListOverviewDto matchListOverview, List<MatchHeaderDto> matchHeaders) {
            Summoner = summoner;
            MatchListOverviewDto = matchListOverview;
            MatchHeaders = matchHeaders;
        }
    }
}