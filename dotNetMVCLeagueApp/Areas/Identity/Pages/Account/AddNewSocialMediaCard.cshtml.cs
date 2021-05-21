using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account {
    public class AddNewSocialMediaCard : PageModel {

        private readonly UserManager<ApplicationUser> userManager;

        public AddNewSocialMediaCard(UserManager<ApplicationUser> userManager) {
            this.userManager = userManager;
        }

        /// <summary>
        /// Obsahuje "legalni" hodnoty pro socialni site, aby uzivatele nemohli zadat jakykoliv odkaz
        /// </summary>
        public List<string> SocialMediaPlatformNames = ServerConstants.SocialMediaPlatformsNames;
        
        public AddNewSocialMediaCardDto Input { get; set; }
        
        /// <summary>
        /// Pocet karet, ktere uzivatel vytvoril
        /// </summary>
        public int UserProfileCards { get; set; }

        public int CardLimit = ServerConstants.CardLimit;
        
        [TempData] public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            var profileCards = user?.ProfileCards.ToList() ?? new();
            UserProfileCards = profileCards.Count;

            return Page();
        }
    }
}