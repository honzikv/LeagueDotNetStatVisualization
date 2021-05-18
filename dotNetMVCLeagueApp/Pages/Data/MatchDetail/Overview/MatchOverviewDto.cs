using System;
using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview {
    public class MatchOverviewDto {
        public bool IsRemake { get; set; }

        /// <summary>
        ///     Hraci ve hre - klic je participant id (1 - 10) a hodnota
        ///     je DTO s informacemi o hraci
        /// </summary>
        public Dictionary<int, PlayerDto> Players { get; set; }

        /// <summary>
        ///     Prvni tym je vzdy tym pro hrace z jehoz profilu jsme
        ///     se na hru dostali.
        /// </summary>
        public MatchTeamsDto Teams { get; set; }

        /// <summary>
        /// Datum hry
        /// </summary>
        public DateTime PlayTime { get; set; }
    }
}