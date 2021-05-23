using dotNetMVCLeagueApp.Data.JsonMappings;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    /// <summary>
    /// Informace o "Queue" uzivatele - zde jsou pouze dve Ranked Solo a Ranked Flex (hodnocene hry)
    /// </summary>
    public class QueueInfoDto {
        
        /// <summary>
        /// Jmeno queue
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Tier - Gold, Silver, Bronze ...
        /// </summary>
        public string Tier { get; set; }
        
        /// <summary>
        /// Rank - I, II, III, IV - vetsi znamena horsi
        /// </summary>
        public string Rank { get; set; }
        
        /// <summary>
        /// Pocet "LP" - body pro postup do vyssiho ranku
        /// </summary>
        public int LeaguePoints { get; set; }
        
        /// <summary>
        /// Celkovy pocet vyher
        /// </summary>
        public int Wins { get; set; }
        
        /// <summary>
        /// Celkovy pocet proher
        /// </summary>
        public int Losses { get; set; }
        
        /// <summary>
        /// % cast vyhranych her
        /// </summary>
        public double Winrate { get; set; }
        
        /// <summary>
        /// Odkaz na asset ranku
        /// </summary>
        public RankAsset RankAsset { get; set; }
    }
}