using System;
using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.PlayerDetail {
    /// <summary>
    ///     Data, ktera jsou spojena s hracem pro ktereho detail zobrazujeme - tzn. z jehoz stranky jsme se na detail dostali
    ///     Klice pro vsechny slovniky jsou ParticipantId
    /// </summary>
    public class PlayerDetailDto {
        public int ParticipantId { get; }

        public int LaneOpponentParticipantId { get; }

        public PlayerDetailDto(int participantId, int laneOpponentParticipantId, IEnumerable<int> participantIds) {
            ParticipantId = participantId;
            LaneOpponentParticipantId = laneOpponentParticipantId;

            foreach (var id in participantIds) {
                if (id == participantId) {
                    continue;
                }

                MaxXpDiff[id] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));
                MaxGoldDiff[id] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));
                MaxCsDiff[id] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));
                MaxLevelDiff[id] = new TimeValue<int>(int.MinValue, TimeSpan.FromSeconds(0));

                MinXpDiff[id] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
                MinGoldDiff[id] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
                MinCsDiff[id] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
                MinLevelDiff[id] = new TimeValue<int>(int.MaxValue, TimeSpan.FromSeconds(0));
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