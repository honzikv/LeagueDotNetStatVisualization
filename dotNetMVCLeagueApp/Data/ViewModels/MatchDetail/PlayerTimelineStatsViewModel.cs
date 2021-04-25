using System.Collections.Generic;
using Castle.Core;

namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail {
    public class PlayerTimelineStatsViewModel {
        /// <summary>
        /// Celkovy pocet XP za cas
        /// </summary>
        public List<int> XpOverTime { get; set; } = new();

        /// <summary>
        /// Celkovy pocet zlata za cas
        /// </summary>
        public List<int> GoldOverTime { get; set; } = new();

        /// <summary>
        /// Celkovy pocet CS za cas
        /// </summary>
        public List<int> CsOverTime { get; set; } = new();

        /// <summary>
        /// Celkova uroven za cas
        /// </summary>
        public List<int> LevelOverTime { get; set; } = new();

    }
}