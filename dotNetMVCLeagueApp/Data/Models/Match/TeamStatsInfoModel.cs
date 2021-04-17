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
        /// Pocet znicenych vezi
        /// </summary>
        public int TowerKills { get; set; }

        /// <summary>
        /// Pocet zabitych heraldu (<=2)
        /// </summary>
        public int RiftHeraldKills { get; set; }

        /// <summary>
        /// Zda-li dal nekdo z tymu prvni zabiti
        /// </summary>
        public bool FirstBlood { get; set; }

        /// <summary>
        /// Pocet znicenych inhibitoru
        /// </summary>
        public int InhibitorKills { get; set; }

        /// <summary>
        /// Zda-li tym zabil prvniho barona
        /// </summary>
        public bool FirstBaron { get; set; }

        /// <summary>
        /// Zda-li tym ziskal prvniho draka
        /// </summary>
        public bool FirstDragon { get; set; }

        /// <summary>
        /// Pocet zabitych draku (<=4)
        /// </summary>
        public int DragonKills { get; set; }

        /// <summary>
        /// Pocet zabitych baronu
        /// </summary>
        public int BaronKills { get; set; }

        /// <summary>
        /// Zda-li tym dostal prvni inhibitor
        /// </summary>
        public bool FirstInhibitor { get; set; }

        /// <summary>
        /// Zda-li tym dostal prvni vez
        /// </summary>
        public bool FirstTower { get; set; }

        /// <summary>
        /// Zda-li tym dostal prvni ho Heralda
        /// </summary>
        public bool FirstRiftHerald { get; set; }
    }
}