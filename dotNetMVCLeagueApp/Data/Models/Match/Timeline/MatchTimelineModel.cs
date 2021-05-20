using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetMVCLeagueApp.Data.Models.Match.Timeline {
    /// <summary>
    ///     Reprezentuje timeline pro jeden zapas
    /// </summary>
    public class MatchTimelineModel {
        
        [ForeignKey("Match")]
        [Required]
        public long Id { get; set; }
        

        /// <summary>
        ///     Doba, jak dlouho trva jeden frame
        /// </summary>
        public long FrameInterval { get; set; }

        /// <summary>
        ///     Reference na vsechny framy zapasu - kazdy frame ma odkaz na frame
        ///     jednotliveho ucastnika a eventy
        /// </summary>
        public virtual IEnumerable<MatchFrameModel> MatchFrames { get; set; }
    }
}