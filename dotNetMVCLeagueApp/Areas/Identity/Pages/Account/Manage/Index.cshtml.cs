using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MingweiSamuel.Camille.SummonerV4;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account.Manage {
    
    /// <summary>
    /// Trida, ktera zpracovava pozadavky z profilu (pouze zobrazuje data)
    /// </summary>
    public partial class IndexModel : PageModel {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Status message
        /// </summary>
        [TempData] public string StatusMessage { get; set; }

        /// <summary>
        /// Uzivatelska data
        /// </summary>
        public ProfileData UserData { get; set; }

        /// <summary>
        /// Uzivatelske jmeno a prirazeny summoner
        /// </summary>
        public class ProfileData {
            /// <summary>
            /// Jmeno uzivatele
            /// </summary>
            [Display(Name = "Username")]
            public string Username { get; set; }

            /// <summary>
            /// Summoner name
            /// </summary>
            [Display(Name = "Summoner Name")]
            public string SummonerName { get; set; }

            /// <summary>
            /// Server, na kterem je summoner
            /// </summary>
            [Display(Name = "Server")]
            public string Server { get; set; }
        }

        /// <summary>
        /// Nacteni dat
        /// </summary>
        /// <param name="user">Prihlaseny uzivatel</param>
        private async Task LoadAsync(ApplicationUser user) {
            var userName = await userManager.GetUserNameAsync(user);

            UserData = user.Summoner is not null
                ? new ProfileData {
                    Username = userName,
                    SummonerName = user.Summoner.Name,
                    Server = user.Summoner.Region
                }
                : new ProfileData {
                    Username = userName
                };
        }

        /// <summary>
        /// GET pro ziskani stranky
        /// </summary>
        /// <returns>Vrati render HTML</returns>
        public async Task<IActionResult> OnGetAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

    }
}