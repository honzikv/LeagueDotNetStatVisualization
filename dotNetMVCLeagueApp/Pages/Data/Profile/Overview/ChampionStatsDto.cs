namespace dotNetMVCLeagueApp.Pages.Data.Profile.Overview {
    /// <summary>
    /// Informace o postave, ktere jsou relevantni pro overview
    /// </summary>
    public class ChampionStatsDto {
        
        /// <summary>
        /// Id postavy
        /// </summary>
        public int ChampionId { get; set; }
        
        public int Kills { get; set; }
        
        public int Deaths { get; set; }
        
        public int Assists { get; set; }

        /// <summary>
        /// Zda-li se jedna o vyhru
        /// </summary>
        public bool Win { get; set; }
        
        public double KillParticipation { get; set; }
        
        public double Kda { get; set; }
        
        public string Role { get; set; }

        public int Gold { get; set; }
        
        public double GoldDiffAt10 { get; set; }
        
        public double TeamGoldShare { get; set; }
        
        public double CsPerMinute { get; set; }
        
        public int Cs { get; set; }
        public double DamageShare { get; set; }
    }
}