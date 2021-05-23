namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    
    public class MatchTimelineOverviewDto {

        public MatchTimelineDto MatchTimeline { get; set; }
        
        public PlayerDetailDto PlayerDetail { get; set; }
        
        /// <summary>
        /// Zda-li je oponent spravne urcen - nakonec nepouzito
        /// </summary>
        public bool IsOpponentAccurate { get; set; }
    }
}