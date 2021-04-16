using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    public class PlayerInfoModel {
        public int Id { get; set; }

        [Required] public virtual MatchInfoModel MatchInfoModel { get; set; }
        [Required] public virtual PlayerStatsModel PlayerStatsModel { get; set; }

        public string SummonerName { get; set; }

        public string SummonerId { get; set; }

        public int ProfileIcon { get; set; }

        public int Spell1Id { get; set; }

        public int Spell2Id { get; set; }

        public int TeamId { get; set; }

        public string Role { get; set; }

        public string Lane { get; set; }
        
        public int ChampionId { get; set; }

        public double? CsPerMinute { get; set; }

        public double? GoldDiffAt10 { get; set; }

        public double? CsDiffAt10 { get; set; }
    }
}