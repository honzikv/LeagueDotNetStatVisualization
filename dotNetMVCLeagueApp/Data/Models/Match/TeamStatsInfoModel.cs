using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    /// <summary>
    /// Statistiky tymu
    /// </summary>
    public class TeamStatsInfoModel {
        public int Id { get; set; }

        /// <summary>
        /// Reference na zapas
        /// </summary>
        [Required]
        public virtual MatchInfoModel MatchInfo { get; set; }

        /// <summary>
        /// 100  - blue side, 200 - red side
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Zda-li se jedna o vyhru
        /// </summary>
        public string Win { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int TowerKills { get; set; }

        public int RiftHeraldKills { get; set; }

        public bool FirstBlood { get; set; }

        public int InhibitorKills { get; set; }

        public bool FirstBaron { get; set; }

        public bool FirstDragon { get; set; }

        public int DragonKills { get; set; }

        public int BaronKills { get; set; }

        public bool FirstInhibitor { get; set; }

        public bool FirstTower { get; set; }

        public bool FirstRiftHerald { get; set; }
    }
}