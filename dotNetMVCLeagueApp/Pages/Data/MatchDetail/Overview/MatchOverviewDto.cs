using System;
using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview {
    public class MatchOverviewDto {
        /// <summary>
        /// Zda-li se jedna o remake
        /// </summary>
        public bool IsRemake { get; set; }
        
        /// <summary>
        /// Typ queue - solo queue, draft pick, blind pick ...
        /// </summary>
        public string QueueType { get; set; }
        
        /// <summary>
        /// Doba trvani hry
        /// </summary>
        public TimeSpan GameDuration { get; set; }
        
        /// <summary>
        /// Zda-li jde pro hrace o vyhru
        /// </summary>
        public bool Win { get; set; }
        
        /// <summary>
        /// Summonername hrace
        /// </summary>
        public string Summoner { get; set; }

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