using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match.Timeline {
    /// <summary>
    ///     Udalost v zapase
    /// </summary>
    public class MatchEventModel {
        /// <summary>
        ///     Id v db
        /// </summary>
        public long Id { get; set; }
        
        [Required]
        public virtual MatchFrameModel MatchFrame { get; set; }

        /// <summary>
        ///     Id ucastnika
        /// </summary>
        public int ParticipantId { get; set; }

        /// <summary>
        ///     Id predmetu (pokud existuje)
        /// </summary>
        public int? ItemId { get; set; }

        /// <summary>
        ///     Typ linky
        /// </summary>
        public string LaneType { get; set; }

        /// <summary>
        ///     Typ eventu
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Cas, kdy se event stal
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        ///     Ten kdo event vytvoril (napr. placing a ward)
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        ///     Typ wardy
        /// </summary>
        public string WardType { get; set; }

        /// <summary>
        ///     Typ monstra - DRAGON, BARON ...
        /// </summary>
        public string MonsterType { get; set; }

        /// <summary>
        ///     Podtyp monstra - WATER_DRAGON, etc.
        /// </summary>
        public string MonsterSubType { get; set; }
        
    }
}