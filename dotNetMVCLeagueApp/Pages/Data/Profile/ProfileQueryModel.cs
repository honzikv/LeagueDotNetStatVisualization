using System.ComponentModel.DataAnnotations;
using dotNetMVCLeagueApp.Config;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    /// <summary>
    /// Data, ktera muze uzivatel hledat - Name a Server jsou povinna
    /// </summary>
    public class ProfileQueryModel {
        
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        public string Server { get; set; }

        public string Filter { get; set; } = ServerConstants.AllGames;

        public int PageSize { get; set; } = ServerConstants.DefaultNumberOfGamesInProfile;

        [Range(0, int.MaxValue, ErrorMessage = "Invalid page number, must be at least {1}")]
        public int PageNumber { get; set; } = 0;
        
    }
}