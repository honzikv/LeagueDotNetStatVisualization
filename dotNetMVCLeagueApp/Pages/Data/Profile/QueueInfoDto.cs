using dotNetMVCLeagueApp.Data.JsonMappings;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    public class QueueInfoDto {
        
        public string Name { get; set; }
        
        public string Tier { get; set; }
        
        public string Rank { get; set; }
        
        public int LeaguePoints { get; set; }
        
        public int Wins { get; set; }
        
        public int Losses { get; set; }
        
        public double Winrate { get; set; }
        
        public RankAsset RankAsset { get; set; }
    }
}