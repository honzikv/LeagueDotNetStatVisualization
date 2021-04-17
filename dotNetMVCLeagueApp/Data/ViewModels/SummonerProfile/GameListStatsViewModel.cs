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

        public Dictionary<int, int> PlayedChampionIds { get; } = new();

        public double AverageKills { get; set; }

        public double AverageDeaths { get; set; }

        public double AverageAssists { get; set; }

        public double AverageKda { get; set; }

        public double AverageKillParticipation { get; set; }

        public string MostPlayedRole { get; set; }

        public string SecondMostPlayedRole { get; set; }

        public double AverageGoldDiffAt10 { get; set; }

        public double AverageCsPerMinute { get; set; }

        /// <summary>
        /// Override pro debug
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{nameof(GamesWon)}: {GamesWon}, {nameof(GamesLost)}: {GamesLost}, {nameof(Remakes)}: {Remakes}, " +
            $"{nameof(PlayedChampionIds)}: {PlayedChampionIds}, {nameof(AverageKills)}: {AverageKills}, " +
            $"{nameof(AverageDeaths)}: {AverageDeaths}, {nameof(AverageAssists)}: {AverageAssists}, " +
            $"{nameof(AverageKda)}: {AverageKda}, {nameof(AverageKillParticipation)}: {AverageKillParticipation}," +
            $"{nameof(MostPlayedRole)}: {MostPlayedRole}, {nameof(SecondMostPlayedRole)}: {SecondMostPlayedRole}, " +
            $"{nameof(AverageGoldDiffAt10)}: {AverageGoldDiffAt10}, {nameof(AverageCsPerMinute)}: {AverageCsPerMinute}";
    }
}