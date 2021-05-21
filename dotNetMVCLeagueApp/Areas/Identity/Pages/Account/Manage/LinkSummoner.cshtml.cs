using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Services;
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

        [BindProperty] public LinkSummonerInputDto LinkSummonerInput { get; set; }

        public Dictionary<string, string> QueryableServers => ServerConstants.QueryableServers;

        public async Task<IActionResult> OnGetAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }

            LinkSummonerInput = user.Summoner is null
                ? new LinkSummonerInputDto()
                : new LinkSummonerInputDto {
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
            if (!QueryableServers.ContainsKey(LinkSummonerInput.Server.ToLower())) {
                StatusMessage = "Error, this server does not exist.";
                return RedirectToPage();
            }

            if (!ModelState.IsValid) {
                return Page();
            }
            
                var operationResult =
                    await summonerService.LinkSummonerToApplicationUser(user, LinkSummonerInput.SummonerName,
                        LinkSummonerInput.Server);


            StatusMessage = operationResult.Message;
            if (!operationResult.Error) {
                await signInManager.RefreshSignInAsync(user); 
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnlinkAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (user.Summoner is not null) {
                user.Summoner = null;
                await userManager.UpdateAsync(user);

                StatusMessage = "Summoner name has been unlinked from your profile.";
            }
            else {
                StatusMessage = "Error, no summoner is linked to your profile.";
            }

            return RedirectToPage();
        }
    }
}