using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    
    /// <summary>
    /// Data pro vytvoreni nove karty s textem
    /// </summary>
    public class AddNewTextCardDto {
        
        /// <summary>
        /// Primarni text karty
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "{0} can be at max {1} characters long.")]
        [Display(Name = "Card Title")]
        public string PrimaryText { get; set; }
        
        /// <summary>
        /// Popisek nebo url
        /// </summary>
        [StringLength(2000, ErrorMessage = "{0} can be at max {1} characters long.")]
        [Display(Name = "Text")]
        public string SecondaryText { get; set; }
        
        /// <summary>
        /// Zda-li se ma karta po vytvoreni zaradit jako prvni
        /// </summary>
        [Required]
        [Display(Name = "Show card as first (on top)")]
        public bool ShowOnTop { get; set; }

    }
}