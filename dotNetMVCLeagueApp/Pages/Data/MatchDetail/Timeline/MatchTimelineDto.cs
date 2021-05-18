using System;
using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    public class MatchTimelineDto {
        public Dictionary<int, PlayerTimelineDto> PlayerTimelines { get; } = new();

        /// <summary>
        ///     Jak dlouho trval jeden frame
        /// </summary>
        public TimeSpan FrameIntervalSeconds { get; }
        
        public MatchTimelineDto(IEnumerable<int> participantIds, TimeSpan frameIntervalSeconds) {
            FrameIntervalSeconds = frameIntervalSeconds;

            // Vytvorime novy objekt pro id
            foreach (var participantId in participantIds)
                PlayerTimelines[participantId] = new PlayerTimelineDto(participantId);
        }
    }
}