using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    public class EditCardDto {
        
        [Required]
        [StringLength(200, ErrorMessage = "Text is too long, it can be at max {1} characters.")]
        public string PrimaryText { get; set; }
        
        public string SecondaryText { get; set; }

        public bool IsSocialMedia { get; set; }
        public int? Id { get; set; }
    }
}