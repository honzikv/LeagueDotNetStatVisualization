using MingweiSamuel.Camille.LeagueV4;

namespace dotNetMVCLeagueApp.Data.Models.SummonerPage {
    public class QueueInfoModel : IEntity {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }
        
        public string QueueType { get; set; }
        
        public string Tier { get; set; }
        
        public string Rank { get; set; }
        
        public int LeaguePoints { get; set; }
        
        public int Wins { get; set; }
        
        public int Losses { get; set; }

    }
}