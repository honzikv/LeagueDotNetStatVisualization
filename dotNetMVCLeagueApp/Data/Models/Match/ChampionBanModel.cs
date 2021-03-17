using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class ChampionBanModel {
        
        /// <summary>
        /// Database id
        /// </summary>
        [Key]
        public int ChampionBanId { get; set; }
        
        /// <summary>
        /// Id of the champion
        /// </summary>
        public int ChampionId { get; set; }
        
        /// <summary>
        /// Turn in the draft (unused)
        /// </summary>
        public int PickTurn { get; set; }
    }
}