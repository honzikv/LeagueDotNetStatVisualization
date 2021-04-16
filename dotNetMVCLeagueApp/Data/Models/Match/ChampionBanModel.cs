using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class ChampionBanModel {
        public long Id { get; set; }

        /// <summary>
        /// Id postavy
        /// </summary>
        public int ChampionId { get; set; }

        /// <summary>
        /// Turn v draftu (nepouzito)
        /// </summary>
        public int PickTurn { get; set; }

        /// <summary>
        /// 100 pro modrou, 200 pro cervenou
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Reference na TeamStatsInfoModel
        /// </summary>
        [Required] public virtual TeamStatsInfoModel TeamStatsInfoModel { get; set; }
    }
}