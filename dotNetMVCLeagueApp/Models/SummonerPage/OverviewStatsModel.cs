using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Models.SummonerPage {
    public class OverviewStatsModel {
        [Key]
        public int OverviewStatsId { get; set; }
        
        /// <summary>
        /// Number of games won shown in the overview list
        /// </summary>
        public int GamesWon { get; set; }
        
        /// <summary>
        /// Number of games lost shown in the overview list
        /// </summary>
        public int GamesLost { get; set; }
        
        /// <summary>
        /// Number of games that were remade shown in the overview list
        /// </summary>
        public int NumberOfRemakes { get; set; }
        
        /// <summary>
        /// Games won / Games lost
        /// </summary>
        public double Winrate { get; set; }
        
        /// <summary>
        /// Riot detected position obtained from the API
        /// </summary>
        public int PrimaryPosition { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int SecondaryPosition { get; set; }
        
        
    }
}