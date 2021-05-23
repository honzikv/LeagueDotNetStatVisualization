using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.User;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    /// <summary>
    /// Objekt s daty pro partial view s tabulkou pro spravu karet (_ProfileCardTablePartial)
    /// </summary>
    public class ProfileCardTableDto {
        
        /// <summary>
        /// Maximalni pocet znaku pro sloupec v tabulce
        /// </summary>
        public int TextTableMaxCharacters = 15;
        
        public string StatusMessage { get; set; }
        public List<ProfileCardModel> ProfileCards { get; set; } = new();
    }
}