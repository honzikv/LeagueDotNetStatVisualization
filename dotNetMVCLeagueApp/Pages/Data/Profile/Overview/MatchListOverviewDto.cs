using System.Collections.Generic;
using dotNetMVCLeagueApp.Config;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview {
    public class MatchListOverviewDto {
        public CumulativeStatsDto TotalStats { get; set; }

        /// <summary>
        /// Pocet remaku (hra se spustila  ale nejaky hrac se na zacatku odpojil a hra skoncila predcasne)
        /// </summary>
        public int Remakes { get; set; } = 0;

        public Dictionary<int, ChampionCumulativeStatsDto> ChampionCumulativeStatsDict { get; } = new();
        
        public int Page { get; set; }

        public int PageSize { get; set; } = ServerConstants.DefaultNumberOfGamesInProfile;
    }
}