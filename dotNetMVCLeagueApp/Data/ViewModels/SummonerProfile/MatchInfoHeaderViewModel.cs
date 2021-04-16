using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile {
    
    /// <summary>
    /// Class that holds information about individual match for specific summoner - only for header
    /// </summary>
    public class MatchInfoHeaderViewModel {

        /// <summary>
        /// Id of the player's team
        /// </summary>
        [NotNull]
        public int TeamId { get; set; }
        
        /// <summary>
        /// Date the game was played
        /// </summary>
        [NotNull]
        public DateTime PlayTime { get; set; }
        
        /// <summary>
        /// Zdali hrac vyhral, prohral popr. remake
        /// </summary>
        [NotNull]
        public string Win { get; set; }
        
        /// <summary>
        /// Type of the queue - blind pick, draft pick, ranked solo and ranked flex
        /// </summary>
        [NotNull]
        public string QueueType { get; set; }
        
        /// <summary>
        /// Items + trinket - this list should always be size of 7
        /// </summary>
        [NotNull]
        public List<int> Items { get; set; }
        
        /// <summary>
        /// Largest achieved multikill - null if only one kill, double, triple ...
        /// </summary>
        public string LargestMultiKill { get; set; }
        
        /// <summary>
        /// Creeps per minute
        /// </summary>
        public double CsPerMinute { get; set; }
        
        /// <summary>
        /// Total creep score
        /// </summary>
        public int TotalCs { get; set; }
        
        /// <summary>
        /// Icon id of the champion the player played
        /// </summary>
        [NotNull]
        public int ChampionIconId { get; set; }
        
        /// <summary>
        /// The first summoner spell
        /// </summary>
        [NotNull]
        public int SummonerSpell1Id { get; set; }
        
        /// <summary>
        /// The second summoner spell
        /// </summary>
        [NotNull]
        public int SummonerSpell2Id { get; set; }
        
        /// <summary>
        /// Vision score
        /// </summary>
        [NotNull]
        public long VisionScore { get; set; }
        
        /// <summary>
        /// Primary rune Id
        /// </summary>
        public int PrimaryRuneId { get; set; }
        
        /// <summary>
        /// Secondary rune Id
        /// </summary>
        public int SecondaryRuneId { get; set; }
    }
}