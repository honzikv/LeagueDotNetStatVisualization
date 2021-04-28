using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.MatchDetail.Overview {
    /// <summary>
    ///     Informace o hraci pro zobrazeni do overview zapasu
    /// </summary>
    public class PlayerInfoDto {
        /// <summary>
        ///     Id ucastnika
        /// </summary>
        public int ParticipantId { get; set; }

        public string SummonerName { get; set; }

        /// <summary>
        ///     Id ikonky pro spravne zobrazeni ve view
        /// </summary>
        public int ChampionId { get; set; }

        /// <summary>
        ///     Jmeno postavy
        /// </summary>
        public string ChampionName { get; set; }

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

        /// <summary>
        ///     Celkovy podil na zabiti v tymu
        /// </summary>
        public double KillParticipation { get; set; }

        /// <summary>
        ///     Seznam predmetu pro id ikonek
        /// </summary>
        public List<int> Items { get; set; } = new();

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
        public int Spell1Id { get; set; }

        /// <summary>
        ///     Id praveho summoner spellu
        /// </summary>
        public int Spell2Id { get; set; }

        /// <summary>
        ///     Celkovy pocet zlata
        /// </summary>
        public int GoldEarned { get; set; }

        /// <summary>
        ///     Celkove poskozeni do ostatnich hracu
        /// </summary>
        public long TotalDamageDealtToChampions { get; set; }

        /// <summary>
        ///     Celkove poskozeni od ostatnich hracu
        /// </summary>
        public long TotalDamageTaken { get; set; }

        /// <summary>
        ///     Celkove "odrazene" poskozeni (napr. pres armor)
        /// </summary>
        public long DamageSelfMitigated { get; set; }
    }
}