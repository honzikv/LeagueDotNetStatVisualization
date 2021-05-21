using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account {
    [AllowAnonymous]
    public class RegisterModel : PageModel {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationUserService applicationUserService;
        private readonly SummonerService summonerService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SummonerService summonerService,
            ApplicationUserService applicationUserService) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.summonerService = summonerService;
            this.applicationUserService = applicationUserService;
            QueryableServers = summonerService.QueryableServers;
        }

        public Dictionary<string, string> QueryableServers { get; }

        [BindProperty] public RegisterDto Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public async Task OnGetAsync(string returnUrl = null) {
            ReturnUrl = returnUrl;
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!ModelState.IsValid) {
                return Page();
            }

            var user = new ApplicationUser {
                UserName = Input.Username,
                Email = Input.Email
            };

            try {
                if (Input.SummonerName is not null) {
                    var region = Region.Get(Input.Server);
                    var summoner = await summonerService.GetSummoner(Input.SummonerName, region)
                                   ?? throw new ActionNotSuccessfulException(
                                       "Summoner does not exist on the specified server");

                    if (await summonerService.IsSummonerTaken(summoner)) {
                        ModelState.AddModelError("SummonerName", "This summoner is already taken");
                    }
                    else {
                        user.Summoner = summoner;
                    }

                }
            }
            catch (Exception ex) {
                ModelState.AddModelError("SummonerName",
                    ex is ActionNotSuccessfulException or RiotApiException
                        ? ex.Message
                        : "Error, while performing registration, please register without summoner and link it later.");
                return Page();
            }

            if (await applicationUserService.IsEmailTaken(Input.Email)) {
                ModelState.AddModelError("Email", "The email is already taken.");
            }

            var result = await userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded) {
                user.EmailConfirmed = true; // nastavime jako potvrzeny email
                await userManager.UpdateAsync(user);
                await signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}