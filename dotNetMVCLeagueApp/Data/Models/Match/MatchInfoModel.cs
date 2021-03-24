using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class MatchInfoModel {
        /// <summary>
        /// Id in database
        /// </summary>
        [Key]
        public int MatchInfoId { get; set; }

        /// <summary>
        /// Whether the player won
        /// </summary>
        public bool Win { get; set; }

        /// <summary>
        /// Id of the team - blue or red
        /// Legal values: 100 === blue, 200 === red
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// List of all bans
        /// </summary>
        public IEnumerable<TeamStatsInfoModel> Teams { get; set; }

        /// <summary>
        /// Date and time when the game was played
        /// </summary>
        public DateTime PlayTime { get; set; }

        /// <summary>
        /// SummonerInfo id reference
        /// </summary>
        [ForeignKey("SummonerInfoModel")]
        public int SummonerInfoId { get; set; }

        public IEnumerable<PlayerInfoModel> PlayerInfoList;
    }
}