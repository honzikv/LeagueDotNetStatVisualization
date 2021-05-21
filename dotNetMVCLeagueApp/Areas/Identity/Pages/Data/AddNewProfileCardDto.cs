using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    public class AddNewProfileCardDto {
        
        [Required]
        [StringLength(100, ErrorMessage = "{0} can be at max {1} characters long.")]
        [Display(Name = "Card Title")]
        public string Header { get; set; }
        
        [StringLength(2000, ErrorMessage = "{0} can be at max {1} characters long.")]
        [Display(Name = "Text")]
        public string Text { get; set; }
        
        [Required]
        [Display(Name = "Show card as first (on top)")]
        public bool ShowOnTop { get; set; }

    }
}