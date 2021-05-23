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
    /// <summary>
    /// Trida pro pridani nove karty se socialni siti
    /// </summary>
    public class AddNewSocialMediaCard : PageModel {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ProfileCardService profileCardService;

        public AddNewSocialMediaCard(UserManager<ApplicationUser> userManager,
            ProfileCardService profileCardService) {
            this.userManager = userManager;
            this.profileCardService = profileCardService;
            CardLimit = profileCardService.CardLimitPerUser;
            SocialMedia = profileCardService.SocialMedia;
        }

        /// <summary>
        /// Obsahuje povolene socialni site pro server
        /// </summary>
        public readonly Dictionary<string, string> SocialMedia;

        /// <summary>
        /// Data pro formular
        /// </summary>
        [BindProperty] public AddNewSocialMediaCardDto Input { get; set; }

        /// <summary>
        /// Pocet karet, ktere uzivatel vytvoril
        /// </summary>
        public int UserProfileCards { get; set; }

        /// <summary>
        /// Maximalni pocet karet pro uzivatele
        /// </summary>
        public readonly int CardLimit;

        [TempData] public string StatusMessage { get; set; }

        /// <summary>
        /// GET pozadavek
        /// </summary>
        /// <returns>Vrati vyrenderovane HTML</returns>
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
        /// POST pozadavek pro formular
        /// </summary>
        /// <returns>Vrati </returns>
        public async Task<IActionResult> OnPostAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            var profileCards = user.ProfileCards?.ToList() ?? new();
            UserProfileCards = profileCards.Count;

            // Nelze vytvorit novou kartu pokud jsme prekrocili limit
            if (UserProfileCards >= CardLimit) {
                StatusMessage = "Error, you cannot create more cards and must delete some first.";
            }

            if (!ModelState.IsValid) {
                return Page();
            }

            // Nyni jeste potrebujeme overit, zda-li se jedna o validni socialni sit
            var operationResult = profileCardService.IsSocialNetworkValid(Input.UserUrl);
            if (operationResult.Error) {
                StatusMessage = operationResult.Message;
                return Page();
            }

            // Vytvorime model a pridame do db
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
            TempData["StatusMessage"] = "New social media card has been sucessfully added.";
            return RedirectToPage("/Account/Manage/ManageProfileCards", new {
                area = "Identity"
            });
        }
    }
}