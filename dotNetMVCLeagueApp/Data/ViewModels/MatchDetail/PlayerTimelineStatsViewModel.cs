using System.Collections.Generic;
using Castle.Core;

namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail {
    public class PlayerTimelineStatsViewModel {
        /// <summary>
        /// Celkovy pocet XP za cas
        /// </summary>
        public IEnumerable<int> XpOverTime { get; set; } = new List<int>();

        public Dictionary<int, int> MinXpParticipants { get; set; } = new();

        public Dictionary<int, int> MaxXpParticipants { get; set; } = new();

        /// <summary>
        /// Celkovy pocet zlata za cas
        /// </summary>
        public IEnumerable<int> GoldOverTime { get; set; } = new List<int>();

        /// <summary>
        /// Nejmene zlata v porovnani s ostatnimi ucastniky
        /// </summary>
        public Dictionary<int, int> MinGoldParticipants { get; set; } = new();

        /// <summary>
        /// Nejvice zlata v porovnani s ostatnimi ucastniky
        /// </summary>
        public Dictionary<int, int> MaxGoldParticipants { get; set; } = new();

        /// <summary>
        /// Celkovy pocet CS za cas
        /// </summary>
        public IEnumerable<int> CsOverTime { get; set; } = new List<int>();

        public Dictionary<int, int> MinCsOverTime { get; set; } = new();
        
        public Dictionary<int, int> MaxCsOverTime { get; set; } = new();

        /// <summary>
        /// Celkova uroven za cas
        /// </summary>
        public IEnumerable<int> LevelOverTime { get; set; } = new List<int>();

    }
}