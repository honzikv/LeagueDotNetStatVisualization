using System.Collections.Generic;
using dotNetMVCLeagueApp.Config;

namespace dotNetMVCLeagueApp.Pages.Data.Profile.Overview {
    public class MatchListOverviewDto {
        public CumulativeStatsDto TotalStats { get; set; }

        /// <summary>
        /// Pocet remaku (hra se spustila  ale nejaky hrac se na zacatku odpojil a hra skoncila predcasne)
        /// </summary>
        public int Remakes { get; set; } = 0;

        public Dictionary<int, ChampionCumulativeStatsDto> ChampionCumulativeStatsDict { get; } = new();
    }
}