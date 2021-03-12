using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace dotNetMVCLeagueApp.Models.Match {
    public class MatchInfoModel {
        /// <summary>
        /// Id in database
        /// </summary>
        [Key]
        public int MatchInfoId { get; set; }

        /// <summary>
        /// True === win
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
        public List<ChampionBanModel> Bans { get; set; }

        [ForeignKey("SummonerInfoModel")]
        public int SummonerInfoId { get; set; }
    }
}