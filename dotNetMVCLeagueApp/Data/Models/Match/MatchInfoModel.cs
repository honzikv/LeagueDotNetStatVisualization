using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class MatchInfoModel {

        /// <summary>
        /// Toto Id je ziskano z Riot API a neni automaticky inkrementovano
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Reference na summonery - M : N
        /// </summary>
        public virtual ICollection<MatchInfoSummonerInfo> SummonerInfoList { get; set; }
        
        /// <summary>
        /// Typ hry - blind pick, draft pick, solo queue nebo flex queue
        /// </summary>
        public string GameType { get; set; }
        
        /// <summary>
        /// Jak dlouho hra trvala
        /// </summary>
        public long GameDuration { get; set; }

        /// <summary>
        /// Seznam vsech banu postav
        /// </summary>
        public virtual IEnumerable<TeamStatsInfoModel> Teams { get; set; }

        /// <summary>
        /// Cas, kdy se hra hrala
        /// </summary>
        public DateTime PlayTime { get; set; }

        /// <summary>
        /// Reference na hrace
        /// </summary>
        public virtual IEnumerable<PlayerInfoModel> PlayerInfoList { get; set; }

    }
}