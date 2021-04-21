using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail {
    public class MatchTimelineStatsViewModel {

        public Dictionary<int, PlayerTimelineStatsViewModel> PlayerStats { get; set; }
        
        public long FrameTime { get; set; }
    }
}