using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class MatchInfoModel {

        /// <summary>
        /// This id is derived from riot api and is not auto incremented
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// M:N relation - several summoners can have the same game
        /// </summary>
        public virtual ICollection<MatchInfoSummonerInfo> SummonerInfoList { get; set; }

        /// <summary>
        /// List of all bans
        /// </summary>
        public virtual IEnumerable<TeamStatsInfoModel> Teams { get; set; }

        /// <summary>
        /// Date and time when the game was played
        /// </summary>
        public DateTime PlayTime { get; set; }

        public virtual IEnumerable<PlayerInfoModel> PlayerInfoList { get; set; }

    }
}