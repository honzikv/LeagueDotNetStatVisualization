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
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly SummonerService summonerService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            SummonerService summonerService,
            IEmailSender emailSender) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.summonerService = summonerService;
            this.emailSender = emailSender;
            QueryableServers = summonerService.QueryableServers;
        }

        public Dictionary<string, string> QueryableServers { get; }

        [BindProperty] public RegisterInputDto RegisterInput { get; set; }

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
                UserName = RegisterInput.Username,
                Email = RegisterInput.Email
            };

            try {
                if (RegisterInput.SummonerName is not null) {
                    var region = Region.Get(RegisterInput.Server);
                    var summoner = await summonerService.GetSummoner(RegisterInput.SummonerName, region);

                    if (await summonerService.IsSummonerTaken(summoner)) {
                        ModelState.AddModelError("SummonerName", "This summoner is already taken");
                    }
                    else {
                        user.Summoner = summoner;
                    }

                }
            }
            catch (RedirectToHomePageException) {
                ModelState.AddModelError("SummonerName", "Summoner does not exist on the specified server");
                return Page();
            }

            var result = await userManager.CreateAsync(user, RegisterInput.Password);
            if (result.Succeeded) {
                // Pro potvrzeni emailu
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new {area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl},
                    protocol: Request.Scheme);

                await emailSender.SendEmailAsync(RegisterInput.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (userManager.Options.SignIn.RequireConfirmedAccount) {
                    return RedirectToPage("RegisterConfirmation", new {email = RegisterInput.Email, returnUrl = returnUrl});
                }

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