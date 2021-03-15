using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Models.Match {
    public class ChampionInfoModel {
        
        [Key]
        public int ChampionInfoId { get; set; }
        
        /// <summary>
        /// Champion's level at the end of the game
        /// </summary>
        public int FinalLevel { get; set; }
        
        /// <summary>
        /// Total creep score
        /// </summary>
        public int TotalCs { get; set; }
        
        /// <summary>
        /// Cs differential at 10th minute - this can be used to determine which player won their lane
        /// </summary>
        public double CsDiffAt10 { get; set; }
        
        /// <summary>
        /// Number of control wards purchased
        /// </summary>
        public int ControlWards { get; set; }
        
        /// <summary>
        /// Vision score - i.e., how much vision to the team did the player provide through wards, ganks, etc.
        /// </summary>
        public int VisionScore { get; set; }
    }
}