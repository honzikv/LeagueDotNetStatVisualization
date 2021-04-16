using System.ComponentModel.DataAnnotations;
using MingweiSamuel.Camille.LeagueV4;

namespace dotNetMVCLeagueApp.Data.Models.SummonerPage {
    public class QueueInfoModel {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Type of queue - can be compared with Const/LeagueEntryConst
        /// </summary>
        public string QueueType { get; set; }

        /// <summary>
        /// i.e. Diamond, Gold, Platinum ...
        /// </summary>
        public string Tier { get; set; }

        /// <summary>
        /// i.e. II, III, IV ...
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// Number of league points
        /// </summary>
        public int LeaguePoints { get; set; }

        /// <summary>
        /// Number of wins
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Number of losses
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Reference to summoner info
        /// </summary>
        [Required]
        public virtual SummonerInfoModel SummonerInfo { get; set; }
    }
}