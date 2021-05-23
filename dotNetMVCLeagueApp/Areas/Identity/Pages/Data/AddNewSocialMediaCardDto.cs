using System.ComponentModel.DataAnnotations;
using static dotNetMVCLeagueApp.Config.ServerConstants;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    
    /// <summary>
    /// Objekt s daty z formulare pro vytvoreni nove karty pro socialni sit
    /// </summary>
    public class AddNewSocialMediaCardDto {

        /// <summary>
        /// Kratky popisek u karty
        /// </summary>
        [Display(Name = "Card Description")]
        [DataType(DataType.Text)]
        [StringLength(CardDescriptionMaxStringLength, ErrorMessage = "{0} can be at max {1} characters long.")]
        [Required]
        public string Description { get; set; }
        
        /// <summary>
        /// URL uzivatele pro odkaz
        /// </summary>
        [Required]
        [StringLength(UserUrlMaxStringLength, ErrorMessage = "{0} can be at max {1} characters long.")]
        [DataType(DataType.Url)]
        [Display(Name = "Social Media URL")]
        public string UserUrl { get; set; }
        
        /// <summary>
        /// Zda-li se ma karta po vytvoreni zaradit jako prvni
        /// </summary>
        [Display(Name = "Show card as first (on top)")]
        public bool ShowOnTop { get; set; }
    }
}