using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account.Manage {
    
    /// <summary>
    /// Trida pro spravu karticek na profil
    /// </summary>
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

        /// <summary>
        /// Status message
        /// </summary>
        [TempData] public string StatusMessage { get; set; }

        /// <summary>
        /// Seznam karticek uzivatele
        /// </summary>
        public List<ProfileCardModel> ProfileCards { get; set; }

        /// <summary>
        /// GET pro ziskani stranky
        /// </summary>
        /// <returns>Vraci HTML stranku</returns>
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

        /// <summary>
        /// Metoda pro smazani karty
        /// </summary>
        /// <param name="cardId">Id karty</param>
        /// <returns>Aktualizovane view</returns>
        public async Task<IActionResult> OnPostDeleteCardAsync([FromForm] int cardId) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            try {
                ProfileCards = await profileCardService.DeleteCard(cardId, user);
                return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                    ProfileCards = ProfileCards
                });
            }

            catch (Exception ex) {
                StatusMessage = ex is ActionNotSuccessfulException
                    ? StatusMessage = ex.Message
                    : "Error while deleting the card.";
            }

            return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                ProfileCards = ProfileCards,
                StatusMessage = StatusMessage
            });
        }

        /// <summary>
        /// Presune danou kartu nahoru v tabulce nahoru
        /// </summary>
        /// <param name="cardId">id karty, kterou chceme posunout</param>
        /// <returns>Aktualizovane view po provedeni operace</returns>
        public async Task<IActionResult> OnPostMoveUpAsync([FromForm] int? cardId) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }
            
            // Seradime karty podle pozice
            ProfileCards = user.ProfileCards?.OrderBy(card => card.Position).ToList() ?? new();

            // ReSharper disable once SimplifyLinqExpressionUseAll
            if (cardId is null || !ProfileCards.Any(card => card.Id == cardId)) {
                StatusMessage = "Error, card position could not be updated";
                return Page();
            }

            // Provedeme posunuti nahoru
            try {
                var updatedCards = await profileCardService.MoveUp((int) cardId, user);
                return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                    ProfileCards = updatedCards
                });
            }
            catch(Exception ex) {
                StatusMessage = ex is ActionNotSuccessfulException
                    ? ex.Message
                    : "Error while changing the position of profile cards.";
            }
            
            // Vratime partial view, ktere se zobrazi pomoci AJAXu
            return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                ProfileCards = ProfileCards,
                StatusMessage = StatusMessage
            });
        }

        /// <summary>
        /// Posune kartu dolu (pokud je to mozne)
        /// </summary>
        /// <param name="cardId">Id karty, kterou cheme posunout</param>
        /// /// <returns>Aktualizovane view po provedeni operace</returns>
        public async Task<IActionResult> OnPostMoveDownAsync([FromForm] int? cardId) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }
            
            logger.LogDebug("Card id is" + cardId);

            ProfileCards = user.ProfileCards?.OrderBy(card => card.Position).ToList() ?? new();
            
            // ReSharper disable once SimplifyLinqExpressionUseAll
            if (cardId is null || !ProfileCards.Any(card => card.Id == cardId)) {
                StatusMessage = "Error, card position could not be updated";
                return Page();
            }

            // Posune kartu dolu
            try {
                var updatedCards = await profileCardService.MoveDown((int) cardId, user);
                return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                    ProfileCards = updatedCards
                });
            }
            catch(Exception ex) {
                StatusMessage = ex is ActionNotSuccessfulException
                    ? ex.Message
                    : "Error while changing the position of profile cards.";
            }

            // Vratime partial view, ktere se zobrazi pomoci AJAXu
            return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                ProfileCards = ProfileCards,
                StatusMessage = StatusMessage
            });
        }

    }
}