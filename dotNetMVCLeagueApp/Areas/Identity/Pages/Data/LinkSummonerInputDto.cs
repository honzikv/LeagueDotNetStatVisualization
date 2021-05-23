using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    
    /// <summary>
    /// Objekt pro pripojeni Summoner uctu
    /// </summary>
    public class LinkSummonerInputDto {
        /// <summary>
        /// Jmeno uctu
        /// </summary>
        [Display(Name = "Summoner name")]
        [DataType(DataType.Text)]
        [Required]
        [MinLength(1)]
        public string SummonerName { get; set; }

        /// <summary>
        /// Server, na kterem je ucet zaregistrovan
        /// </summary>
        [Display(Name = "Server")] [Required] public string Server { get; set; }
    }
}