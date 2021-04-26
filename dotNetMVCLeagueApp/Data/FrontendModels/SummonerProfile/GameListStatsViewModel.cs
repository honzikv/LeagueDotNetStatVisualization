using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile {
    /// <summary>
    ///     Tato trida obsahuje statistiky vypoctene pro dany seznam her - tzn. napr. z 20 zobrazenych her na uzivatelskem
    ///     profilu
    /// </summary>
    public class GameListStatsViewModel {
        /// <summary>
        ///     Pocet vyhranych her
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        ///     Pocet prohranych her
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        ///     Pocet nedohranych her
        /// </summary>
        public int Remakes { get; set; }

        /// <summary>
        ///     Hrane postavy
        ///     TODO: implement
        /// </summary>
        public Dictionary<int, int> PlayedChampionIds { get; } = new();

        /// <summary>
        ///     Prumerne zabiti
        /// </summary>
        public double AverageKills { get; set; }

        /// <summary>
        ///     Prumerne smrti
        /// </summary>
        public double AverageDeaths { get; set; }

        /// <summary>
        ///     Prumerne asistenci
        /// </summary>
        public double AverageAssists { get; set; }

        /// <summary>
        ///     Prumerne KDA
        /// </summary>
        public double AverageKda { get; set; }

        /// <summary>
        ///     Prumerna spoluucast na zabitich
        /// </summary>
        public double AverageKillParticipation { get; set; }

        /// <summary>
        ///     Nejhranejsi role
        /// </summary>
        public string MostPlayedRole { get; set; }

        /// <summary>
        ///     Druha nejhranejsi role
        /// </summary>
        public string SecondMostPlayedRole { get; set; }

        /// <summary>
        ///     Prumerne zlata
        /// </summary>
        public double AverageGold { get; set; }

        /// <summary>
        ///     Maximum zlata
        /// </summary>
        public double MaxGold { get; set; }

        /// <summary>
        ///     Prumerny rozdil zlata v 10 minute
        /// </summary>
        public double AverageGoldDiffAt10 { get; set; }

        /// <summary>
        ///     Prumerne CS za minutu
        /// </summary>
        public double AverageCsPerMinute { get; set; }

        /// <summary>
        ///     Override pro debug
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"{nameof(GamesWon)}: {GamesWon}, {nameof(GamesLost)}: {GamesLost}, {nameof(Remakes)}: {Remakes}, " +
                   $"{nameof(PlayedChampionIds)}: {PlayedChampionIds}, {nameof(AverageKills)}: {AverageKills}, " +
                   $"{nameof(AverageDeaths)}: {AverageDeaths}, {nameof(AverageAssists)}: {AverageAssists}, " +
                   $"{nameof(AverageKda)}: {AverageKda}, {nameof(AverageKillParticipation)}: {AverageKillParticipation}," +
                   $"{nameof(MostPlayedRole)}: {MostPlayedRole}, {nameof(SecondMostPlayedRole)}: {SecondMostPlayedRole}, " +
                   $"{nameof(AverageGoldDiffAt10)}: {AverageGoldDiffAt10}, {nameof(AverageCsPerMinute)}: {AverageCsPerMinute}";
        }
    }
}