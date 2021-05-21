using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail {
    public class MatchQueryDto {
        
        /// <summary>
        /// Id hry - pro ziskani z db
        /// </summary>
        [Required]
        public long GameId { get; set; }
        
        /// <summary>
        /// Server, pro ktery hru hledame
        /// </summary>
        [Required]
        public string Server { get; set; }
        
        /// <summary>
        /// Id ucastnika
        /// </summary>
        [Required]
        [Range(1, 10)] // Maximalni rozsah pro hry co hledame bude vzdy 1 - 10
        public int ParticipantId { get; set; }
        
        /// <summary>
        /// Summoner name, ze ktereho hledame
        /// </summary>
        [Required]
        public string SummonerName { get; set; }
        
    }
}