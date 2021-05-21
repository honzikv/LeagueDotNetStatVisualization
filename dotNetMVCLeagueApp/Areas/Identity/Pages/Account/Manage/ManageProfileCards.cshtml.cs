using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account.Manage {
    public class ManageProfileCards : PageModel {
        private readonly ProfileCardService profileCardService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ManageProfileCards> logger;

        public ManageProfileCards(ProfileCardService profileCardService,
            UserManager<ApplicationUser> userManager,
            ILogger<ManageProfileCards> logger) {
            this.profileCardService = profileCardService;
            this.userManager = userManager;
            this.logger = logger;
        }

        [TempData] public string StatusMessage { get; set; }

        public List<ProfileCardModel> ProfileCards { get; set; }


        public async Task<IActionResult> OnGetAsync() {
            logger.LogDebug("OnGetAsync");
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            // Pokud ma uzivatel nejake profilove karty ulozime je, jinak vytvorime prazdny seznam
            ProfileCards = user.ProfileCards?.OrderBy(card => card.Position).ToList() ?? new();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteCardAsync([FromForm] int cardId) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            try {
                ProfileCards = await profileCardService.DeleteCard(cardId, user);
            }

            catch (Exception ex) {
                StatusMessage = ex is ActionNotSuccessfulException
                    ? StatusMessage = ex.Message
                    : "Error while deleting the card.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostMoveUpAsync([FromForm] int? cardId) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            ProfileCards = user.ProfileCards?.OrderBy(card => card.Position).ToList() ?? new();

            // ReSharper disable once SimplifyLinqExpressionUseAll
            if (cardId is null || !ProfileCards.Any(card => card.Id == cardId)) {
                StatusMessage = "Error, card position could not be updated";
                return Page();
            }

            
        }

        public async Task<IActionResult> OnPostMoveDownAsync([FromForm] int? cardId) { }

        public async Task<IActionResult> OnPostReorderCardsAsync([FromForm] int[] cardPositions) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            if (cardPositions is null) {
                StatusMessage = "Error, card position could not be updated";
                return Page();
            }

            var isUnique = cardPositions.Length == cardPositions.Distinct().Count();

            if (!isUnique) {
                StatusMessage = "Error, invalid card identifiers.";
                return Page();
            }

            try {
                var updatedProfileCards =
                    await profileCardService.UpdateProfileCards(cardPositions, user);
                ProfileCards = updatedProfileCards;
            }
            catch (Exception ex) {
                StatusMessage = ex is ActionNotSuccessfulException
                    ? ex.Message
                    : "Error while changing the position of profile cards.";
            }

            return Page();
        }
    }
}