using System;
using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.Match;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    /// <summary>
    ///     Data, ktera jsou spojena s hracem pro ktereho detail zobrazujeme - tzn. z jehoz stranky jsme se na detail dostali
    ///     Klice pro vsechny slovniky jsou ParticipantId
    /// </summary>
    public class PlayerDetailDto {
        [JsonIgnore]
        public PlayerModel Player { get; }

        [JsonIgnore]
        public PlayerModel LaneOpponentParticipantId { get; }

        public PlayerDetailDto(PlayerModel player, PlayerModel laneOpponent, IEnumerable<PlayerModel> opponents) {
            Player = player;
            LaneOpponentParticipantId = laneOpponent;

            foreach (var playerModel in opponents) {
                if (Player.ParticipantId == playerModel.ParticipantId) {
                    continue;
                }

                MaxXpDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));
                MaxGoldDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));
                MaxCsDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));
                MaxLevelDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));

                MinXpDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
                MinGoldDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
                MinCsDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
                MinLevelDiff[playerModel.ParticipantId] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
            }
        }

        /// <summary>
        ///     Rozdil zlata oproti ostatnim hracum v 10 minute
        /// </summary>
        public Dictionary<int, int> GoldDiffAt10 { get; } = new();

        /// <summary>
        ///     Rozdil zlata oproti ostatnim hracum v 15 minute
        /// </summary>
        public Dictionary<int, int> GoldDiffAt15 { get; } = new();

        /// <summary>
        ///     Rozdil XP v 10 minute oproti ostatnim hracum
        /// </summary>
        public Dictionary<int, int> XpDiffAt10 { get; } = new();

        /// <summary>
        ///     Rozdil XP v 15 minute oproti ostatnim hracum
        /// </summary>
        public Dictionary<int, int> XpDiffAt15 { get; } = new();

        /// <summary>
        ///     Rozdil urovne oproti ostatnim hracum v 10 minute
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> LevelDiffAt10 { get; } = new();

        /// <summary>
        ///     Rozdil urovne oproti ostatnim hracum v 15 minute
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> LevelDiffAt15 { get; } = new();

        /// <summary>
        ///     Rozdil CS v 10 minute
        /// </summary>
        public Dictionary<int, int> CsDiffAt10 { get; } = new();

        /// <summary>
        ///     Rozdil CS v 15 minute
        /// </summary>
        public Dictionary<int, int> CsDiffAt15 { get; } = new();

        /// <summary>
        ///     Maximalni rozdil XP oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MaxXpDiff { get; } = new();

        /// <summary>
        ///     Minimalni rozdil XP oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MinXpDiff { get; } = new();

        /// <summary>
        ///     Maximalni rozdil zlata oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MaxGoldDiff { get; } = new();

        /// <summary>
        ///     Minimalni rozdil zlata oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MinGoldDiff { get; } = new();

        /// <summary>
        ///     Maximalni rozdil CS oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MinCsDiff { get; } = new();

        /// <summary>
        ///     Minimalni rozdil cs oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MaxCsDiff { get; } = new();

        /// <summary>
        /// Maximalni rozdil urovne oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MaxLevelDiff { get; } = new();

        /// <summary>
        /// Minimalni rozdil urovne oproti danemu hraci
        /// </summary>
        public Dictionary<int, TimeValue<int>> MinLevelDiff { get; } = new();
    }
}