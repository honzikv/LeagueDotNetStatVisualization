using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.Models.Champion {
    
    /// <summary>
    /// Tato trida slouzi k ulozeni zaznamu o championovi pro daneho hrace za jednu hru - aby sla data lepe ziskavat
    /// z databaze
    /// </summary>
    public class ChampionMatchRecord {
        /// <summary>
        /// Id pro DB
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Zda-li se jedna o Win - muze byt i remake, proto neni boolean
        /// </summary>
        public string Win { get; set; }

        /// <summary>
        /// Id postavy vzhledem k Riot Api
        /// </summary>
        public int ChampionId { get; set; }

        /// <summary>
        /// Pocet zabiti
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        /// Pocet smrti
        /// </summary>
        public int Deaths { get; set; }
        
        /// <summary>
        /// Pocet asistenci
        /// </summary>
        public int Assists { get; set; }
        
        /// <summary>
        /// Gold
        /// </summary>
        public int Gold { get; set; }
        
        /// <summary>
        /// Creep score
        /// </summary>
        public int Cs { get; set; }
        
        /// <summary>
        /// Poskozeni do hracu
        /// </summary>
        public int DamageToPlayers { get; set; }
        
        /// <summary>
        /// Reference na hrace
        /// </summary>
        public virtual SummonerInfoModel Summoner { get; set; }
    }
}