using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match.Timeline {
    
    /// <summary>
    /// Reprezentuje jeden zaznam pro frame daneho uzivatele
    /// </summary>
    public class MatchParticipantFrameModel {
        
        /// <summary>
        /// Id by melo byt radeji long protoze techto dat bude opravdu hodne
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 1 - 10
        /// </summary>
        public int ParticipantId { get; set; }
        
        [Required]
        public virtual MapPositionModel Position { get; set; }
        
        /// <summary>
        /// Pocet aktualne zabitych minionu
        /// </summary>
        public int MinionsKilled { get; set; }
        
        /// <summary>
        /// Zabitych monster v jungle
        /// </summary>
        public int JungleMinionsKilled { get; set; }
        
        /// <summary>
        /// Celkove zlata od zacatku hry
        /// </summary>
        public int TotalGold { get; set; }
        
        /// <summary>
        /// Aktualne zlata - unspent gold
        /// </summary>
        public int CurrentGold { get; set; }
        
        /// <summary>
        /// Uroven postavy
        /// </summary>
        public int Level { get; set; }
        
        
    }
}