using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account {
    /// <summary>
    /// Trida pro odhlaseni (vytvoreno scaffoldingem)
    /// </summary>
    [AllowAnonymous]
    public class LogoutModel : PageModel {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LogoutModel> logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger) {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPost(string returnUrl = null) {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out.");
            if (returnUrl != null) {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage();
        }
    }
}