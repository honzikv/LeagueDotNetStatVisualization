using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail {
    public class MatchTimelineStatsViewModel {

        public List<PlayerTimelineStatsViewModel> PlayerStats { get; }
        
        /// <summary>
        /// Jak dlouho trval jeden frame
        /// </summary>
        public long FrameInterval { get; }

        public MatchTimelineStatsViewModel(int numParticipants, long frameInterval) {
            PlayerStats = new List<PlayerTimelineStatsViewModel>(numParticipants);
            FrameInterval = frameInterval;
        }
    }
}