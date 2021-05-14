using System.ComponentModel.DataAnnotations;
using dotNetMVCLeagueApp.Const;

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

        public int PageNumber { get; set; } = 0;
    }
}