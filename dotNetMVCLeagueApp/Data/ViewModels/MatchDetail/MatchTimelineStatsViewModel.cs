using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail {
    public class MatchTimelineStatsViewModel {

        /// <summary>
        /// Obsahuje data vsech ucastniku, krome toho, se kterym porovnavame
        /// </summary>
        private readonly List<PlayerTimelineStatsViewModel> playerStats;

        private readonly List<PlayerEventViewModel> playerEvents;
        
        public OpponentStatsViewModel OpponentStats { get; set; }

        /// <summary>
        /// Id ucastnika, pro ktereho se data zobrazuji
        /// </summary>
        public int ParticipantId { get; set; }
        
        /// <summary>
        /// Jak dlouho trval jeden frame
        /// </summary>
        public long FrameInterval { get; }

        public MatchTimelineStatsViewModel(int numParticipants, long frameInterval) {
            playerStats = new List<PlayerTimelineStatsViewModel>(numParticipants);
            FrameInterval = frameInterval;
            
            for (var i = 0; i < numParticipants; i += 1) {
                playerStats.Add(new());
            }
        }

        public PlayerTimelineStatsViewModel GetParticipant(int participantId) => playerStats[participantId - 1];
    }

}