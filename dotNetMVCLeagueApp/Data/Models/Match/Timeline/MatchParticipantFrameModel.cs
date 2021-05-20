using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match.Timeline {
    /// <summary>
    ///     Reprezentuje jeden zaznam pro frame daneho uzivatele
    /// </summary>
    public class MatchParticipantFrameModel {
        /// <summary>
        ///     Id
        /// </summary>
        public long Id { get; set; }
        
        [Required]
        public virtual MatchFrameModel MatchFrame { get; set; }

        /// <summary>
        ///     1 - 10
        /// </summary>
        public int ParticipantId { get; set; }

        /// <summary>
        ///     Pocet aktualne zabitych minionu
        /// </summary>
        public int MinionsKilled { get; set; }

        /// <summary>
        ///     Zabitych monster v jungle
        /// </summary>
        public int JungleMinionsKilled { get; set; }

        /// <summary>
        ///     Celkove zlata od zacatku hry
        /// </summary>
        public int TotalGold { get; set; }

        /// <summary>
        ///     Aktualni XP
        /// </summary>
        public int Xp { get; set; }

        /// <summary>
        ///     Uroven postavy
        /// </summary>
        public int Level { get; set; }
        
    }
}