﻿using System;
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
            
            return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                ProfileCards = ProfileCards,
                StatusMessage = StatusMessage
            });
        }

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

            return Partial("_ProfileCardTablePartial", new ProfileCardTableDto {
                ProfileCards = ProfileCards,
                StatusMessage = StatusMessage
            });
        }

    }
}