using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.Match;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    /// <summary>
    ///     Obsahuje vybrane timeline pro jednoho hrace.
    /// </summary>
    public class PlayerTimelineDto {
        public PlayerTimelineDto(PlayerModel player) {
            ParticipantId = player.ParticipantId;
            SummonerName = player.SummonerName;
        }

        /// <summary>
        ///     Id ucastnika
        /// </summary>
        public int ParticipantId { get; set; }
        
        /// <summary>
        /// Summoner name
        /// </summary>
        public string SummonerName { get; set; }

        /// <summary>
        ///     Celkovy pocet XP za cas
        /// </summary>
        public List<int> XpOverTime { get; } = new();

        /// <summary>
        ///     Celkovy pocet zlata za cas
        /// </summary>
        public List<int> GoldOverTime { get; } = new();

        /// <summary>
        ///     Celkovy pocet CS za cas
        /// </summary>
        public List<int> CsOverTime { get; } = new();

        /// <summary>
        ///     Celkova uroven za cas
        /// </summary>
        public List<int> LevelOverTime { get; } = new();
    }
}