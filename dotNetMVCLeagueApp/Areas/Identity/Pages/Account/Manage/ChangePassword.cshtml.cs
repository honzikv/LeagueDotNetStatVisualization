using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account.Manage {
    
    /// <summary>
    /// Trida pro zpracovani zmeny hesla
    /// </summary>
    public class ChangePasswordModel : PageModel {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ChangePasswordModel> logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        /// <summary>
        /// Data pro registraci
        /// </summary>
        [BindProperty] public InputModel Input { get; set; }

        /// <summary>
        /// Zprava pro uzivatele 
        /// </summary>
        [TempData] public string StatusMessage { get; set; }

        /// <summary>
        /// Obsahuje data pro registraci
        /// </summary>
        public class InputModel {
            
            /// <summary>
            /// Stare heslo
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            /// <summary>
            /// Nove heslo
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            /// Potvrzeni hesla
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// Get pozadavek
        /// </summary>
        /// <returns>Vrati render HTML stranky</returns>
        public async Task<IActionResult> OnGetAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var hasPassword = await userManager.HasPasswordAsync(user);
            if (!hasPassword) {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        /// <summary>
        /// Post formulare pro zmenu hesla
        /// </summary>
        /// <returns>Vrati render HTML stranky</returns>
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            // Ziskame uzivatele
            var user = await userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            // Provedeme zmenu hesla
            var changePasswordResult =
                await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded) {
                foreach (var error in changePasswordResult.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            // refresh loginu
            await signInManager.RefreshSignInAsync(user);
            logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }
    }
}