using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    /// <summary>
    ///     Informace o hraci v danem zapasu
    /// </summary>
    public class PlayerModel {
        public int Id { get; set; }

        /// <summary>
        ///     Reference na zapas
        /// </summary>
        [Required]
        public virtual MatchModel MatchModel { get; set; }

        /// <summary>
        ///     Reference na statistiku
        /// </summary>
        [Required]
        public virtual PlayerStatsModel PlayerStatsModel { get; set; }

        /// <summary>
        ///     Participant id pro referenci na timeline
        /// </summary>
        public int ParticipantId { get; set; }

        /// <summary>
        ///     Uzivatelske jmeno
        /// </summary>
        public string SummonerName { get; set; }

        /// <summary>
        ///     Encrypted summoner id
        /// </summary>
        public string SummonerId { get; set; }

        /// <summary>
        ///     Ikonka profilu
        /// </summary>
        public int ProfileIcon { get; set; }

        /// <summary>
        ///     Id leveho summoner spellu
        /// </summary>
        public int Spell1Id { get; set; }

        /// <summary>
        ///     Id praveho summoner spellu
        /// </summary>
        public int Spell2Id { get; set; }

        /// <summary>
        ///     Id tymu - 100 nebo 200
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        ///     Role (viz GameConstants)
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        ///     Linka (viz GameConstants)
        /// </summary>
        public string Lane { get; set; }

        /// <summary>
        ///     Id postavy
        /// </summary>
        public int ChampionId { get; set; }

        /// <summary>
        ///     Rozdil zlata oproti oponentovi v 10 minute - muze byt null
        /// </summary>
        public double? GoldDiffAt10 { get; set; }

        /// <summary>
        ///     Rozdil cs v 10 minute
        /// </summary>
        public double? CsDiffAt10 { get; set; }
    }
}