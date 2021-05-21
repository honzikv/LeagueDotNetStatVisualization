using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    public class AddNewSocialMediaCardDto {
        
        /// <summary>
        /// Vybrana platforma, pro kterou se bude karta zobrazovat - Twitter, Youtube, Reddit ...
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Invalid social platform provided")]
        [Display(Name = "Social Platform")]
        public string SocialPlatform { get; set; }
        
        /// <summary>
        /// Kratky popisek u karty
        /// </summary>
        [Display(Name = "Card Description (max 200 chars)")]
        [DataType(DataType.Text)]
        [StringLength(200, ErrorMessage = "{0} can be at max {1} characters long.")]
        public string Description { get; set; }
        
        /// <summary>
        /// URL uzivatele pro odkaz
        /// </summary>
        [Required]
        [StringLength(2000, ErrorMessage = "{0} can be at max {1} characters long.")]
        [DataType(DataType.Url)]
        [Display(Name = "Profile URL")]
        public string UserUrl { get; set; }
        
        /// <summary>
        /// Zda-li se ma karta po vytvoreni zaradit jako prvni
        /// </summary>
        [Display(Name = "Show card as first (on top)")]
        public bool ShowOnTop { get; set; }
    }
}