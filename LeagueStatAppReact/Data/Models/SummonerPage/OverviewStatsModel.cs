﻿namespace LeagueStatAppReact.Data.Models.SummonerPage {
    public class OverviewStatsModel : IEntity {

        public int Id { get; set; }

        /// <summary>
        /// Number of games won
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        /// Number of games lost
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        /// Number of games that were remade
        /// </summary>
        public int NumberOfRemakes { get; set; }

        /// <summary>
        /// Games won / Games lost
        /// </summary>
        public double Winrate { get; set; }

        /// <summary>
        /// The most played position throughout all displayed games
        /// </summary>
        public int PrimaryPosition { get; set; }

        /// <summary>
        /// The second most played position throughout all displayed games
        /// </summary>
        public int SecondaryPosition { get; set; }

    }
}