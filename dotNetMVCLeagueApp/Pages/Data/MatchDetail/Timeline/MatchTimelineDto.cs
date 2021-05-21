using System;
using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.Match;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    public class MatchTimelineDto {
        public Dictionary<int, PlayerTimelineDto> PlayerTimelines { get; } = new();

        /// <summary>
        ///     Jak dlouho trval jeden frame
        /// </summary>
        public TimeSpan FrameIntervalSeconds { get; }
        
        public MatchTimelineDto(IEnumerable<PlayerModel> players, TimeSpan frameIntervalSeconds) {
            FrameIntervalSeconds = frameIntervalSeconds;

            // Vytvorime novy objekt pro id
            foreach (var player in players)
                PlayerTimelines[player.ParticipantId] = new PlayerTimelineDto(player);
        }
    }
}