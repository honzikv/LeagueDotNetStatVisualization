using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner {
    public class SummonerOverviewDto {
        
        
        public string QueueType { get; }

        public SummonerProfileDto Summoner { get; }
        
        public MatchListDto MatchList { get; }

        public SummonerOverviewDto(SummonerProfileDto summoner, string queueType, MatchListOverviewDto matchListOverview, List<MatchHeaderDto> matchHeaders) {
            MatchList = new MatchListDto(matchHeaders, matchListOverview);
            QueueType = queueType;
            Summoner = summoner;
        }
    }

}