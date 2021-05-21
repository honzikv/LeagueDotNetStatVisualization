using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.User;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    public class ProfileCardTable {
        
        /// <summary>
        /// Maximalni pocet znaku pro sloupec v tabulce
        /// </summary>
        public int TextTableMaxCharacters = 20;
        public List<ProfileCardModel> ProfileCards { get; set; }
    }
}