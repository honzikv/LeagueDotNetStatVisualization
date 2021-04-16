using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile {
    
    /// <summary>
    /// Tato trida obsahuje statistiky vypoctene pro dany seznam her - tzn. napr. z 20 zobrazenych her na uzivatelskem
    /// profilu
    /// </summary>
    public class GameListStatsViewModel {
        
        public int GamesWon { get; set; }
        
        public int GamesLost { get; set; }
        
        public int Remakes { get; set; }

        public List<int> PlayedChampionIds { get; } = new();
        
        public int AverageKills { get; set; }
        
        public int AverageDeaths { get; set; }
        
        public int AverageAssists { get; set; }
        
        public double AverageKda { get; set; }
        
        public double AverageKillParticipation { get; set; }
        
        public string MostPlayedRole { get; set; }
        
        public string SecondMostPlayedRole { get; set; }
        
        public double AverageGoldDiffAt10 { get; set; }

        public double AverageCsPerMinute { get; set; }
        
    }
}