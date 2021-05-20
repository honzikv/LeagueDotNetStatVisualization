using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    public class UpdateProfileCardPositionsDto {
        
        /// <summary>
        /// Pozice karet
        /// </summary>
        [Range(0, int.MaxValue)]
        public int[] CardPositions { get; set; }
    }

}