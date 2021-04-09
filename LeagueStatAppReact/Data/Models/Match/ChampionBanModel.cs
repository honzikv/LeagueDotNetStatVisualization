using System.ComponentModel.DataAnnotations;

namespace LeagueStatAppReact.Data.Models.Match {
    public class ChampionBanModel : IEntity {
        public int Id { get; set; }

        /// <summary>
        /// Id of the champion
        /// </summary>
        public int ChampionId { get; set; }

        /// <summary>
        /// Turn in the draft (unused)
        /// </summary>
        public int PickTurn { get; set; }

        /// <summary>
        /// Either 100 - for blue, or 200 - for red
        /// </summary>
        public int TeamId { get; set; }

        [Required] public virtual TeamStatsInfoModel TeamStatsInfoModel { get; set; }
    }
}