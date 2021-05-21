using System.Collections.Generic;
using dotNetMVCLeagueApp.Pages.Data.Profile.Overview;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    public class SummonerOverviewDto {

        public SummonerProfileDto Summoner { get; }

        public List<MatchHeaderDto> MatchHeaders { get; }

        public MatchListOverviewDto MatchListOverview { get; }

        public SummonerOverviewDto(SummonerProfileDto summoner, ProfileQueryDto profileQueryDto,
            MatchListOverviewDto matchListOverview, List<MatchHeaderDto> matchHeaders
        ) {
            MatchHeaders = matchHeaders;
            MatchListOverview = matchListOverview;
            Summoner = summoner;
        }
    }
}