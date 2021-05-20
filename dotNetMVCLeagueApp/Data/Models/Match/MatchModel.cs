using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class MatchModel {
        /// <summary>
        ///     Toto Id je ziskano z Riot API a neni automaticky inkrementovano
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Reference na summonery - M : N
        /// </summary>
        public virtual ICollection<MatchToSummonerModel> SummonerInfoList { get; set; }

        /// <summary>
        ///     Typ hry - blind pick, draft pick, solo queue nebo flex queue
        /// </summary>
        public string QueueType { get; set; }

        /// <summary>
        ///     Jak dlouho hra trvala v s
        /// </summary>
        public long GameDuration { get; set; }
        
        /// <summary>
        /// Zda-li se uz v API hledalo match history - nektere hry nemuseji mit match history nahranou
        /// a nemusime api zbytecne prohledavat vicekrat
        /// </summary>
        public bool MatchTimelineSearched { get; set; }

        /// <summary>
        ///     Seznam vsech banu postav
        /// </summary>
        public virtual IEnumerable<TeamStatsModel> Teams { get; set; }

        /// <summary>
        ///     Cas, kdy se hra hrala
        /// </summary>
        public DateTime PlayTime { get; set; }

        /// <summary>
        ///     Reference na hrace
        /// </summary>
        public virtual ICollection<PlayerModel> PlayerList { get; set; }
        
        /// <summary>
        /// Reference na timeline, muze byt null
        /// </summary>
        public virtual MatchTimelineModel MatchTimeline { get; set; }
    }
}