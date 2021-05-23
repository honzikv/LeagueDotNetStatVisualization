using System;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account {
    public class AddNewCard : PageModel {
        
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ProfileCardService profileCardService;

        public AddNewCard(UserManager<ApplicationUser> userManager, ProfileCardService profileCardService) {
            this.userManager = userManager;
            this.profileCardService = profileCardService;
            CardLimit = profileCardService.CardLimitPerUser;
        }

        [BindProperty] public AddNewTextCardDto Input { get; set; }
        
        [TempData] public string StatusMessage { get; set; }
        
        /// <summary>
        /// Pocet karet, ktere uzivatel vytvoril
        /// </summary>
        public int UserProfileCards { get; set; }

        /// <summary>
        /// Limit karet, ktere je mozne vytvorit
        /// </summary>
        public readonly int CardLimit;

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

            try {
                var profileCard = new ProfileCardModel {
                    SocialMedia = false,
                    PrimaryText = Input.PrimaryText,
                    SecondaryText = Input.SecondaryText,
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