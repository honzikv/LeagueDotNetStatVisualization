using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Data.Models.Match;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    public class MatchTimelineDto {
        public Dictionary<int, PlayerTimelineDto> PlayerTimelines { get; } = new();
        
        public List<int> ParticipantIds { get; }

        /// <summary>
        ///     Jak dlouho trval jeden frame
        /// </summary>
        public TimeSpan FrameIntervalSeconds { get; }
        
        public MatchTimelineDto(List<PlayerModel> players, TimeSpan frameIntervalSeconds) {
            FrameIntervalSeconds = frameIntervalSeconds;
            ParticipantIds = players.Select(player => player.ParticipantId).ToList();

            // Vytvorime novy objekt pro id
            foreach (var player in players)
                PlayerTimelines[player.ParticipantId] = new PlayerTimelineDto(player);
        }
    }
}