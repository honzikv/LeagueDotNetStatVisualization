using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    /// <summary>
    /// Objekt s daty pro upravu karty
    /// </summary>
    public class EditCardDto {
        
        /// <summary>
        /// Nadpis / popisek
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Text is too long, it can be at max {1} characters.")]
        public string PrimaryText { get; set; }
        
        /// <summary>
        /// Popisek (u textu) nebo URL (u socialni site)
        /// </summary>
        public string SecondaryText { get; set; }

        /// <summary>
        /// Zda-li se jedna o socialni sit
        /// </summary>
        public bool IsSocialMedia { get; set; }
        
        /// <summary>
        /// Id karty
        /// </summary>
        public int? Id { get; set; }
    }
}