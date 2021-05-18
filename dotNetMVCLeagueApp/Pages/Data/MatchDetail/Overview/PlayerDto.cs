using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.JsonMappings;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview {
    /// <summary>
    ///     Informace o hraci pro zobrazeni do overview zapasu
    /// </summary>
    public class PlayerDto {
        /// <summary>
        ///     Id ucastnika
        /// </summary>
        public int ParticipantId { get; set; }
        
        /// <summary>
        /// Id tymu - 100 nebo 200
        /// </summary>
        public int TeamId { get; set; }

        public string SummonerName { get; set; }

        /// <summary>
        ///     Id ikonky pro spravne zobrazeni ve view
        /// </summary>
        public int ChampionId { get; set; }

        /// <summary>
        ///     Jmeno postavy
        /// </summary>
        public ChampionAsset ChampionAsset { get; set; }

        /// <summary>
        ///     Pocet zabiti
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        ///     Pocet smrti
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        ///     Pocet asistenci
        /// </summary>
        public int Assists { get; set; }
        
        public double Kda { get; set; }

        /// <summary>
        ///     Celkovy podil na zabiti v tymu
        /// </summary>
        public double KillParticipation { get; set; }

        /// <summary>
        ///     Seznam predmetu pro id ikonek
        /// </summary>
        public List<ItemAsset> Items { get; set; }

        /// <summary>
        ///     Pocet zabitych jednotek
        /// </summary>
        public int Cs { get; set; }

        /// <summary>
        ///     Cs za minutu
        /// </summary>
        public double CsPerMinute { get; set; }

        /// <summary>
        ///     Id leveho summoner spellu
        /// </summary>
        public SummonerSpellAsset SummonerSpell1 { get; set; }

        /// <summary>
        ///     Id praveho summoner spellu
        /// </summary>
        public SummonerSpellAsset SummonerSpell2 { get; set; }
        
        /// <summary>
        ///     Vision score - kolik vize hrac poskytl svemu tymu
        /// </summary>
        public long VisionScore { get; set; }
        
        /// <summary>
        ///     Primary rune Id
        /// </summary>
        public RuneAsset PrimaryRune{ get; set; }

        /// <summary>
        ///     Secondary rune Id
        /// </summary>
        public RuneAsset SecondaryRune { get; set; }

        /// <summary>
        ///     Celkovy pocet zlata
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        ///     Celkove poskozeni do ostatnich hracu
        /// </summary>
        public long DamageDealt { get; set; }

        /// <summary>
        ///     Rozdil zlata v 10 minute
        /// </summary>
        public double? GoldDiffAt10 { get; set; }
        
        /// <summary>
        ///     Rozdil zlata v 20 minute
        /// </summary>
        public double? GoldDiffAt20 { get; set; }
        
        /// <summary>
        ///     Rozdil cs v 10 minute
        /// </summary>
        public double? CsDiffAt10 { get; set; }

        /// <summary>
        /// Rozdil cs v 20 minute
        /// </summary>
        public double? CsDiffAt20 { get; set; }
        
        /// <summary>
        /// Uroven postavy
        /// </summary>
        public int Level { get; set; }
    }
}