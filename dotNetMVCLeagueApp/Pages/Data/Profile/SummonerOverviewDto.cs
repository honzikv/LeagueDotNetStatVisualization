using System.Collections.Generic;
using dotNetMVCLeagueApp.Pages.Data.Profile.Overview;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    
    /// <summary>
    /// Objekt s daty pro profil
    /// </summary>
    public class SummonerOverviewDto {

        /// <summary>
        /// Data o summoneru
        /// </summary>
        public SummonerProfileDto Summoner { get; }

        /// <summary>
        /// Data pro karty se zapasy
        /// </summary>
        public List<MatchHeaderDto> MatchHeaders { get; }

        /// <summary>
        /// Data pro statistiku ze zobrazenych karet
        /// </summary>
        public MatchListOverviewDto MatchListOverview { get; }

        public SummonerOverviewDto(SummonerProfileDto summoner,
            MatchListOverviewDto matchListOverview, List<MatchHeaderDto> matchHeaders
        ) {
            MatchHeaders = matchHeaders;
            MatchListOverview = matchListOverview;
            Summoner = summoner;
        }
    }
}