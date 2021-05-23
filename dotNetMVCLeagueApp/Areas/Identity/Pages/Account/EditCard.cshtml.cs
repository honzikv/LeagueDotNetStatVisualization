using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account {
    public class EditCard : PageModel {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ProfileCardService profileCardService;
        private readonly ILogger<EditCard> logger;

        public EditCard(UserManager<ApplicationUser> userManager, ProfileCardService profileCardService,
            ILogger<EditCard> logger) {
            this.userManager = userManager;
            this.profileCardService = profileCardService;
            this.logger = logger;
            this.SocialMedia = profileCardService.SocialMedia;
        }

        /// <summary>
        /// Post data pro upravu karty
        /// </summary>
        [BindProperty]
        public EditCardDto Input { get; set; }

        /// <summary>
        /// Obsahuje "legalni" hodnoty pro socialni site, aby uzivatele nemohli zadat jakykoliv odkaz
        /// </summary>
        public readonly List<string> SocialMediaPlatformNames = ServerConstants.SocialMediaPlatformsNames;

        /// <summary>
        /// Chyby pri validaci
        /// </summary>
        public readonly Dictionary<string, string> ValidationErrors = new();

        [TempData] public string StatusMessage { get; set; }
        public readonly Dictionary<string, string> SocialMedia;

        public async Task<IActionResult> OnGetAsync(int cardId) {
            logger.LogDebug($"received cardId: {cardId}");
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            var card = await profileCardService.GetCard(cardId, user);

            if (card is null) {
                TempData["StatusMessage"] = "Error, no such card that could be edited exists";
                return RedirectToPage("/Account/Manage/ManageProfileCards", new {
                    area = "Identity"
                });
            }

            Input = new() {
                Id = card.Id,
                IsSocialMedia = card.SocialMedia,
                PrimaryText = card.PrimaryText,
                SecondaryText = card.SecondaryText
            };

            return Page();
        }

        private (bool, List<ValidationResult>) IsValid() {
            var validationResults = new List<ValidationResult>();
            if (Input.IsSocialMedia) {
                var socialMediaCard = new AddNewSocialMediaCardDto {
                    Description = Input.PrimaryText,
                    UserUrl = Input.SecondaryText
                };

                return (Validator.TryValidateObject(socialMediaCard, new(socialMediaCard), validationResults, true),
                    validationResults);
            }

            var textCard = new AddNewTextCardDto {
                PrimaryText = Input.PrimaryText,
                SecondaryText = Input.SecondaryText
            };

            return (Validator.TryValidateObject(textCard, new(textCard), validationResults, true),
                validationResults);
        }

        public async Task<IActionResult> OnPostAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            if (Input is null || Input.Id is null or 0) {
                TempData["StatusMessage"] = "Error, no such card that could be edited exists";
                return RedirectToPage("/Account/Manage/ManageProfileCards", new {
                    area = "Identity"
                });
            }

            // Nejsnazsi reseni je namapovat vstup na AddNewSocialMediaCardDto nebo AddNewTextCardDto
            // a zvalidovat je pomoci validatoru. Pokud validator vrati ze je vse v poradku provedeme update
            // entity, jinak vratime chyby.
            // Entity namapujeme v metode isValid, ktera vrati zda-li jsou validni a pripadne vysledky validace
            var (isValid, validationResults) = IsValid();

            // Pokud neni validni zapiseme chyby a vratime stranku
            if (!isValid) {
                MapValidationErrors(validationResults);
                return Page();
            }

            // Jeste musime zkontrolovat ze je spravne url pokud se jedna o link
            if (Input.IsSocialMedia) {
                var operationResult = profileCardService.IsSocialNetworkValid(Input.SecondaryText);
                if (operationResult.Error) {
                    StatusMessage = operationResult.Message;
                    return Page();
                }
            }

            // Jinak ulozime do db a presmerujeme na spravu karet
            // Check na id jsme provedli na zacatku takze muzeme pretypovat na int
            var profileCard = await profileCardService.GetCard((int) Input.Id, user);
            if (profileCard is null) {
                TempData["StatusMessage"] = "Error, no such card that could be edited exists";
                return RedirectToPage("/Account/Manage/ManageProfileCards", new {
                    area = "Identity"
                });
            }

            // Namapujeme kartu
            profileCard.PrimaryText = Input.PrimaryText;
            profileCard.SecondaryText = Input.SecondaryText;
            profileCard.SocialMedia = Input.IsSocialMedia;

            TempData["StatusMessage"] = "Card has been successfully updated.";
            try {
                await profileCardService.UpdateCard(profileCard);
            }
            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                TempData["StatusMessage"] = "Error, no card could not be updated.";
            }

            return RedirectToPage("/Account/Manage/ManageProfileCards", new {
                area = "Identity"
            });
        }

        /// <summary>
        /// Funkce, ktera ulozi do slovniku ValidationErrors vsechny chyby
        /// </summary>
        /// <param name="validationResults">Vysledek validace</param>
        private void MapValidationErrors(List<ValidationResult> validationResults) {
            foreach (var validationResult in validationResults) {
                if (validationResult.ErrorMessage is not null) {
                    foreach (var property in validationResult.MemberNames) {
                        logger.LogInformation($"property: {property}, error: {validationResult.ErrorMessage}");
                        ValidationErrors[property] = validationResult.ErrorMessage;
                    }
                }
            }
        }
    }
}