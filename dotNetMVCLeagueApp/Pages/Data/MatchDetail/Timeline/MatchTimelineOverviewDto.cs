namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    public class MatchTimelineOverviewDto {
        
        public string StatusMessage { get; set; }
        
        public MatchTimelineDto MatchTimeline { get; set; }
        
        public PlayerDetailDto PlayerDetail { get; set; }
        
        public bool IsOpponentAccurate { get; set; }
    }
}