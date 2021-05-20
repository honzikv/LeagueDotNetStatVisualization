using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotNetMVCLeagueApp.Areas.Identity.Controllers {
    public class ManageProfileCardsController : Controller {
        private readonly ProfileCardService profileCardService;
        private readonly UserManager<ApplicationUser> userManager;

        public ManageProfileCardsController(ProfileCardService profileCardService,
            UserManager<ApplicationUser> userManager) {
            this.profileCardService = profileCardService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            return View("Account/Manage/ManageProfileCards", new ManageProfileCardsDto {
                ProfileCards = user.ProfileCards?.ToList() ?? new()
            });
        }

        [HttpPost]
        public async Task<IActionResult> OnDeleteCard([FromForm] int cardId) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            var cards = await profileCardService.DeleteCard(cardId, user);
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> OnUpdatePagePositions(
            [FromForm] UpdateProfileCardPositionsDto updateProfileCardPositionsDto) {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            if (!ModelState.IsValid) {
                return View("Account/Manage/ManageProfileCards", new ManageProfileCardsDto {
                    ProfileCards = user.ProfileCards?.ToList() ?? new(),
                    StatusMessage = "Error, no ids were sent"
                });
            }

            var containsUnique = updateProfileCardPositionsDto.CardPositions.Length ==
                                 updateProfileCardPositionsDto.CardPositions.Distinct().Count();
            if (!containsUnique) {
                return View("Account/Manage/ManageProfileCards", new ManageProfileCardsDto {
                    ProfileCards = user.ProfileCards?.ToList() ?? new(),
                    StatusMessage = "Error, form does not contain unique ids"
                });
            }

            try {
                var updatedProfileCards =
                    await profileCardService.UpdateProfileCards(updateProfileCardPositionsDto.CardPositions, user);

                return View("Account/Manage/ManageProfileCards", new ManageProfileCardsDto {
                    ProfileCards = updatedProfileCards,
                    StatusMessage = "Profile Cards were sucessfully updated."
                });

            }
            catch (Exception ex) {
                return View("Account/Manage/ManageProfileCards", new ManageProfileCardsDto {
                    ProfileCards = user.ProfileCards?.ToList() ?? new(),
                    StatusMessage = ex is ActionNotSuccessfulException
                        ? ex.Message
                        : "Error while changing the position of profile cards."
                });
            }
        }
    }
}