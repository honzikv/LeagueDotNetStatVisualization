﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Services.Summoner;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account.Manage {
    public partial class LinkSummoner : PageModel {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly SummonerService summonerService;

        private readonly ILogger<LinkSummoner> logger;

        public LinkSummoner(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SummonerService summonerService,
            ILogger<LinkSummoner> logger) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.summonerService = summonerService;
        }

        [TempData] public string StatusMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; }

        public Dictionary<string, string> QueryableServers => ServerConstants.QueryableServers;

        public class InputModel {
            [Display(Name = "Summoner name")]
            [DataType(DataType.Text)]
            [Required]
            public string SummonerName { get; set; }

            [Display(Name = "Server")] [Required] public string Server { get; set; }
        }

        public async Task<IActionResult> OnGetAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            Input = user.Summoner is null
                ? new InputModel()
                : new InputModel {
                    SummonerName = user.Summoner.Name,
                    Server = user.Summoner.Region
                };

            return Page();
        }

        /// <summary>
        /// Metoda pro handling postu formulare
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            // Validace serveru, pokud neexistuje vratime status message ze neexistuje
            // Toto by se melo stat pouze tehdy, pokud si nekdo umyslne upravi POST request mimo html
            if (!QueryableServers.ContainsKey(Input.Server.ToLower())) {
                StatusMessage = "Error, this server does not exist.";
                return RedirectToPage();
            }

            if (!ModelState.IsValid) {
                user = await userManager.GetUserAsync(User);
            }

            if (Input.SummonerName != user.Summoner.Name) {
                var summoner = summonerService.GetSummonerAsync(Input.SummonerName,
                    Region.Get(Input.Server.ToUpper()));

                var isSummonerTaken = await summonerService.IsSummonerTaken(summoner);

                // Pokud je summoner zabrany a jeho encryptedSummonerId neni stejne jako pro tohoto uzivatele
                // tak vratime chybu, ze je summoner zabrany
                if (isSummonerTaken && summoner.EncryptedSummonerId != user.Summoner.EncryptedSummonerId) {
                    StatusMessage = "Error, this summoner is already taken.";
                    return RedirectToPage();
                }

                // Jinak zmenime summonera a aktualizujeme
                user.Summoner = summoner;
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded) {
                    StatusMessage = "Error while saving the data.";
                    return RedirectToPage();
                }
            }

            await signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated.";
            return RedirectToPage();
        }
    }
}