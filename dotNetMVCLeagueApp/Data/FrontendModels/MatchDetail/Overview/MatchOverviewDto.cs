using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail.Overview {
    public class MatchOverviewDto {
        /// <summary>
        ///     Hraci ve hre - klic je participant id (1 - 10) a hodnota
        ///     je DTO s informacemi o hraci
        /// </summary>
        public Dictionary<int, PlayerInfoDto> Players;

        /// <summary>
        ///     Prvni tym je vzdy tym pro hrace z jehoz profilu jsme
        ///     se na hru dostali.
        /// </summary>
        public List<TeamInfoDto> Teams;
    }
}