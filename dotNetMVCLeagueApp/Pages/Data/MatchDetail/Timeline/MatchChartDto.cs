using System.Collections.Generic;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline {
    /// <summary>
    /// Dto / Model pro _MatchChartPartial
    /// </summary>
    public class MatchChartDto {
        public List<PlayerDto> BlueSidePlayers { get; set; }
        public List<PlayerDto> RedSidePlayers { get; set; }
        
        public List<string> ParticipantColors { get; set; }
        
        /// <summary>
        /// Id, pomoci ktereho budeme nastavovat id ve view, aby je slo pozdeji referencovat pres
        /// JQuery
        /// </summary>
        public string ChartId { get; set; }
    }
}