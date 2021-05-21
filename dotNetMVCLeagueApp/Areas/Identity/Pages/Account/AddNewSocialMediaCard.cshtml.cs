using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account {
    public class AddNewSocialMediaCard : PageModel {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ProfileCardService profileCardService;

        public AddNewSocialMediaCard(UserManager<ApplicationUser> userManager,
            ProfileCardService profileCardService) {
            this.userManager = userManager;
            this.profileCardService = profileCardService;
        }

        /// <summary>
        /// Obsahuje "legalni" hodnoty pro socialni site, aby uzivatele nemohli zadat jakykoliv odkaz
        /// </summary>
        public readonly List<string> SocialMediaPlatformNames = ServerConstants.SocialMediaPlatformsNames;

        public readonly Dictionary<string, string> SocialMediaUrlPrefixes = ServerConstants.SocialMediaPlatformPrefixes;

        [BindProperty] public AddNewSocialMediaCardDto Input { get; set; }

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

            var profileCards = user.ProfileCards?.ToList() ?? new();
            UserProfileCards = profileCards.Count;

            if (UserProfileCards >= CardLimit) {
                StatusMessage = "Error, you cannot create more cards and must delete some first.";
            }

            return Page();
        }

        /// <summary>
        /// Zkontroluje, zda-li je URL link na socialni sit validni
        /// </summary>
        /// <returns>
        /// dvojici bool a string, kdy bool indikuje zda-li je validni a string je bud null nebo
        /// not null pri chybe
        /// </returns>
        private (bool, string) IsSocialNetworkValid() {
            if (!SocialMediaUrlPrefixes.ContainsKey(Input.SocialPlatform)) {
                return (false, "Invalid Social platform.");
            }

            var socialMediaUrlPrefix = SocialMediaUrlPrefixes[Input.SocialPlatform];
            return !Input.UserUrl.StartsWith(socialMediaUrlPrefix, StringComparison.InvariantCultureIgnoreCase)
                ? (false, $"Invalid url, must start with {socialMediaUrlPrefix}")
                : (true, null);
        }

        public async Task<IActionResult> OnPostAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            var profileCards = user.ProfileCards?.ToList() ?? new();
            UserProfileCards = profileCards.Count;

            if (UserProfileCards >= CardLimit) {
                StatusMessage = "Error, you cannot create more cards and must delete some first.";
            }

            if (!ModelState.IsValid) {
                return Page();
            }

            var (isUrlValid, errorMessage) = IsSocialNetworkValid();
            if (!isUrlValid) {
                StatusMessage = errorMessage;
                return Page();
            }

            try {
                var profileCard = new ProfileCardModel {
                    SocialMedia = true,
                    PrimaryText = Input.Description,
                    SecondaryText = Input.UserUrl,
                    ApplicationUser = user
                };
                await profileCardService.Add(profileCard, Input.ShowOnTop, user);
            }
            catch (Exception ex) {
                // Pokud dojde k nejake chybe zustaneme na strance
                StatusMessage = ex is ActionNotSuccessfulException
                    ? StatusMessage = ex.Message
                    : "Error while adding the card.";
                return Page();
            }

            // Pokud se vse podarilo presmerujeme uzivatele na manage profile cards, ktere zobrazi
            // aktualizovany seznam karet
            TempData["StatusMessage"] = "New text card has been sucessfully added.";
            return RedirectToPage("/Account/Manage/ManageProfileCards", new {
                area = "Identity"
            });
        }
    }
}