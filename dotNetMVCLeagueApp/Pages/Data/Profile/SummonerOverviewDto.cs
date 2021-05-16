using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    public class SummonerOverviewDto {

        public SummonerProfileDto Summoner { get; }

        public List<MatchHeaderDto> MatchHeaders { get; }

        public MatchListOverviewDto MatchListOverview { get; }

        public SummonerOverviewDto(SummonerProfileDto summoner, ProfileQueryModel profileQueryModel,
            MatchListOverviewDto matchListOverview, List<MatchHeaderDto> matchHeaders
        ) {
            MatchHeaders = matchHeaders;
            MatchListOverview = matchListOverview;
            Summoner = summoner;
        }
    }
}