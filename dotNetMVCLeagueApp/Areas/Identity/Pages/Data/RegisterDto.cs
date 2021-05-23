using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    /// <summary>
    /// Objekt s daty pro registraci
    /// </summary>
    public class RegisterDto {
        
        /// <summary>
        /// Uzivatelske jmeno
        /// </summary>
        [Required]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 4)]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Heslo
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Potvrzeni hesla
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Server uzivatelskeho uctu (muze byt null)
        /// </summary>
        [Display(Name = "Server")] public string Server { get; set; }

        /// <summary>
        /// Uzivatelske jmeno - summoner name
        /// </summary>
        [Display(Name = "Summoner name")]
        [DataType(DataType.Text)]
        public string SummonerName { get; set; }
    }
}