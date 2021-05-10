using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Services.Summoner;
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
        private readonly SummonerInfoService summonerInfoService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            SummonerInfoService summonerInfoService,
            IEmailSender emailSender) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.summonerInfoService = summonerInfoService;
            this.emailSender = emailSender;
            QueryableServers = summonerInfoService.GetQueryableServers;
        }

        public Dictionary<string, string> QueryableServers { get; }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// Objekt pro formular
        /// </summary>
        public class InputModel {
            [Required]
            [Display(Name = "Username")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 4)]
            [DataType(DataType.Text)]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Server")] public string Server { get; set; }

            [Display(Name = "Summoner name")]
            [DataType(DataType.Text)]
            public string SummonerName { get; set; }
        }

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
                    var summoner = summonerInfoService.GetSummonerInfoAsync(Input.SummonerName, region);

                    if (await summonerInfoService.IsSummonerTaken(summoner)) {
                        ModelState.AddModelError("SummonerName", "This summoner is already taken");
                    }

                    user.Summoner = summoner;
                }
            }
            catch (RedirectToHomePageException) {
                ModelState.AddModelError("SummonerName", "Summoner does not exist on the specified server");
                return Page();
            }

            var result = await userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded) {
                // Pro potvrzeni emailu
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new {area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl},
                    protocol: Request.Scheme);

                await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (userManager.Options.SignIn.RequireConfirmedAccount) {
                    return RedirectToPage("RegisterConfirmation", new {email = Input.Email, returnUrl = returnUrl});
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