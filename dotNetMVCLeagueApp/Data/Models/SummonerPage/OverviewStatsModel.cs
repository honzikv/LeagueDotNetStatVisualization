using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.SummonerPage {
    public class OverviewStatsModel {

        public int Id { get; set; }

        /// <summary>
        /// Pocet vyhranych her
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        /// Pocet prohranych her
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        /// Pocet her, ktere byly "remake" - tzn spustily se ale nedohraly
        /// </summary>
        public int NumberOfRemakes { get; set; }

        /// <summary>
        /// Pocet vyhranych / prohranym hram
        /// </summary>
        public double Winrate { get; set; }

        /// <summary>
        /// Primarni pozice hrace - TOP, MID, JUNGLE, ADC, SUP
        /// </summary>
        public int PrimaryPosition { get; set; }

        /// <summary>
        /// Sekundarni pozice hrace (null pokud je 100% her primarnich)
        /// </summary>
        public int? SecondaryPosition { get; set; }

    }
}