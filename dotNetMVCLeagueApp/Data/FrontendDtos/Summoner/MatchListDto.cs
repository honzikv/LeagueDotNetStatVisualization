using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner {
    public class MatchListDto {
        public List<MatchHeaderDto> MatchHeaders { get; }

        public MatchListOverviewDto MatchListOverview { get; }

        public MatchListDto(List<MatchHeaderDto> matchHeaders, MatchListOverviewDto matchListOverview) {
            MatchHeaders = matchHeaders;
            MatchListOverview = matchListOverview;
        }
    }
}