using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class TeamStatsInfoModel : IEntity {
        public int Id { get; set; }
        
        [ForeignKey("MatchInfoModel")]
        public int MatchInfoId { get; set; }
        
        public IEnumerable<ChampionBanModel> Bans;

        public int TeamId { get; set; }

        public string Win { get; set; }
        
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