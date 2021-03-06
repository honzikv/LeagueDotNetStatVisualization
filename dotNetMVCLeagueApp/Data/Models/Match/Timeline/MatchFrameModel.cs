using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match.Timeline {
    /// <summary>
    ///     Jeden frame v zapase, obsahuje udalosti a framy pro kazdeho
    ///     participanta
    /// </summary>
    public class MatchFrameModel {
        public long Id { get; set; }

        /// <summary>
        ///     Casove  razitko - pro serazeni z db
        /// </summary>
        public long Timestamp { get; set; }
        
        /// <summary>
        /// Reference na casovou osu - pro cascade mazani v db
        /// </summary>
        [Required]
        public virtual MatchTimelineModel MatchTimeline { get; set; }

        /// <summary>
        ///     Framy pro kazdeho ucastnika - 1 - 10
        /// </summary>
        public virtual IEnumerable<MatchParticipantFrameModel> ParticipantFrames { get; set; }

        /// <summary>
        /// Reference na vsechny udalosti (nakonec nepouzito)
        /// </summary>
        public virtual IEnumerable<MatchEventModel> MatchEvents { get; set; }
    }
}