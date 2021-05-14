using System;
using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.JsonMappings;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner {
    /// <summary>
    ///     Trida, ktera obsahuje informace pro hlavicku pro dany zapas
    /// </summary>
    public class MatchHeaderDto {
        /// <summary>
        ///     Id tymu - blue 100, red 200
        /// </summary>
        public int TeamId { get; set; }
        
        /// <summary>
        /// Jak dlouho hra trvala
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///     Datum hry
        /// </summary>
        public TimeSpan PlayTime { get; set; }

        /// <summary>
        ///     Zdali hrac vyhral, prohral popr. remake
        /// </summary>
        public bool Win { get; set; }

        /// <summary>
        ///     Detekovana role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        ///     Poskozeni do hracu
        /// </summary>
        public long DamageDealt { get; set; }

        /// <summary>
        ///     Pocet zabiti
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        ///     Pocet asistenci
        /// </summary>
        public int Assists { get; set; }

        /// <summary>
        ///     Pocet smrti
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        ///     Pocet zabiti a asistenci deleno poctem smrti
        /// </summary>
        public double Kda { get; set; }

        /// <summary>
        ///     Ucast na zabitich v ramci tymu
        /// </summary>
        public double KillParticipation { get; set; }

        /// <summary>
        ///     Typ queue - Ranked solo, Ranked duo ...
        /// </summary>
        public string QueueType { get; set; }

        /// <summary>
        ///     itemy + trinket
        /// </summary>
        public List<ItemAsset> Items { get; set; }

        /// <summary>
        ///     Nejvetsi multi kill
        /// </summary>
        public string LargestMultiKill { get; set; }

        /// <summary>
        ///     CS za minutu
        /// </summary>
        public double CsPerMinute { get; set; }

        /// <summary>
        ///     Celkovy pocet CS
        /// </summary>
        public int TotalCs { get; set; }

        /// <summary>
        ///     Ikonka postavy, kterou hrac hral
        /// </summary>
        public ChampionAsset ChampionAsset { get; set; }

        /// <summary>
        ///     Prvni summoner spell
        /// </summary>
        public SummonerSpellAsset SummonerSpell1 { get; set; }

        /// <summary>
        ///     Druhy summoner spell
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
        
        public int Gold { get; set; }

    }
}