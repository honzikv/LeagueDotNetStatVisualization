using System;
using System.Collections.Generic;

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
        ///     Datum hry
        /// </summary>
        public DateTime PlayTime { get; set; }

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
        public List<int> Items { get; set; }

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
        public int ChampionIconId { get; set; }

        /// <summary>
        ///     Prvni summoner spell
        /// </summary>
        public int SummonerSpell1Id { get; set; }

        /// <summary>
        ///     Druhy summoner spell
        /// </summary>
        public int SummonerSpell2Id { get; set; }

        /// <summary>
        ///     Vision score - kolik vize hrac poskytl svemu tymu
        /// </summary>
        public long VisionScore { get; set; }

        /// <summary>
        ///     Primary rune Id
        /// </summary>
        public int PrimaryRuneId { get; set; }

        /// <summary>
        ///     Secondary rune Id
        /// </summary>
        public int SecondaryRuneId { get; set; }

        /// <summary>
        ///     Override pro debug
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"{nameof(TeamId)}: {TeamId}, {nameof(PlayTime)}: {PlayTime}, {nameof(Win)}: {Win}, " +
                   $"{nameof(QueueType)}: {QueueType}, {nameof(Items)}: {Items}, {nameof(LargestMultiKill)}: " +
                   $"{LargestMultiKill}, {nameof(CsPerMinute)}: {CsPerMinute}, {nameof(TotalCs)}: {TotalCs}, " +
                   $"{nameof(ChampionIconId)}: {ChampionIconId}, {nameof(SummonerSpell1Id)}: {SummonerSpell1Id}, " +
                   $"{nameof(SummonerSpell2Id)}: {SummonerSpell2Id}, {nameof(VisionScore)}: {VisionScore}, " +
                   $"{nameof(PrimaryRuneId)}: {PrimaryRuneId}, {nameof(SecondaryRuneId)}: {SecondaryRuneId}";
        }
    }
}