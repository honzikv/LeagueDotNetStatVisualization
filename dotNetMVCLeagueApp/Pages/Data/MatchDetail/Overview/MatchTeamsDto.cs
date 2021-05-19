using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview {
    public class MatchTeamsDto {
        
        /// <summary>
        /// Blue side - modry tym
        /// </summary>
        public TeamDto BlueSide { get; set; }

        /// <summary>
        /// Hraci pro blue side
        /// </summary>
        public Dictionary<int, PlayerDto> BlueSidePlayers { get; set;  }
        
        /// <summary>
        /// Red side - cerveny tym
        /// </summary>
        public TeamDto RedSide { get; set; }
        
        /// <summary>
        /// Hraci pro red side
        /// </summary>
        public Dictionary<int, PlayerDto> RedSidePlayers { get; set; }
    }
}