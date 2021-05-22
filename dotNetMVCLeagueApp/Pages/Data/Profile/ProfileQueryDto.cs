using System.ComponentModel.DataAnnotations;
using dotNetMVCLeagueApp.Config;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    /// <summary>
    /// Data, ktera muze uzivatel hledat - Name a Server jsou povinna
    /// </summary>
    public class ProfileQueryDto {
        
        /// <summary>
        /// Jmeno uzivatele na serveru, napr. "Renekton Gaming"
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        /// <summary>
        /// Jmeno serveru, napr. "EUW" nebo "euw"
        /// </summary>
        [Required]
        public string Server { get; set; }

        /// <summary>
        /// Typ filtru - napr. "ALL_GAMES" nebo "RANKED_FLEX_SR"
        /// </summary>
        public string Filter { get; set; } = ServerConstants.AllGames;

        /// <summary>
        /// Velikost stranky - kolik her zobrazime, napr. 10
        /// </summary>
        public int PageSize { get; set; } = ServerConstants.DefaultPageSize;

        /// <summary>
        /// Offset - kolik her preskocime 
        /// </summary>
        [Range(0, ServerConstants.GamesLimit, ErrorMessage = "Invalid page offset, must be at least 0 at max 200")]
        public int Offset { get; set; }
        
    }
}