using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    
    public class LinkSummonerInputDto {
        [Display(Name = "Summoner name")]
        [DataType(DataType.Text)]
        [Required]
        [MinLength(1)]
        public string SummonerName { get; set; }

        [Display(Name = "Server")] [Required] public string Server { get; set; }
    }
}