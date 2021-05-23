using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account.Manage {
    
    /// <summary>
    /// Trida pro zpracovani zmeny emailu
    /// </summary>
    public partial class EmailModel : PageModel {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public EmailModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender) {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        /// <summary>
        /// Email uzivatele
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Zda-li je email potvrzeny
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// Status zprava pro uzivatele
        /// </summary>
        [TempData] public string StatusMessage { get; set; }

        /// <summary>
        /// Data z formulare
        /// </summary>
        [BindProperty] public InputModel Input { get; set; }

        /// <summary>
        /// Data pro formular
        /// </summary>
        public class InputModel {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        /// <summary>
        /// Nacteni dat uzivatele
        /// </summary>
        /// <param name="user">Prihlaseny uzivatel</param>
        private async Task LoadAsync(ApplicationUser user) {
            var email = await userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel {
                NewEmail = email,
            };

            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        }

        /// <summary>
        /// Get pro ziskani stranky
        /// </summary>
        /// <returns>Vraci render HTML</returns>
        public async Task<IActionResult> OnGetAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        /// <summary>
        /// Post pro odeslani formulare na zmenu emailu
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostChangeEmailAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid) {
                await LoadAsync(user);
                return Page();
            }

            var email = await userManager.GetEmailAsync(user);
            if (Input.NewEmail != email) {
                var userId = await userManager.GetUserIdAsync(user);
                var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new {userId = userId, email = Input.NewEmail, code = code},
                    protocol: Request.Scheme);
                await emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        #region verificationEmail

#if false
        public async Task<IActionResult> OnPostSendVerificationEmailAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
        
            if (!ModelState.IsValid) {
                await LoadAsync(user);
                return Page();
            }
        
            var userId = await userManager.GetUserIdAsync(user);
            var email = await userManager.GetEmailAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new {area = "Identity", userId = userId, code = code},
                protocol: Request.Scheme);
            await emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        
            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
#endif

        #endregion
    }
}