using System.ComponentModel.DataAnnotations;
using dotNetMVCLeagueApp.Data.Models.Match;

namespace dotNetMVCLeagueApp.Data.Models.User {
    
    /// <summary>
    /// Entita, ktera reprezentuje poznamku k zapasu
    /// Obsahuje referenci na uzivatele (pres nej lze ziskat summoner info) a na zapas
    /// </summary>
    public class MatchNote {
        public int Id { get; set; }
        
        [MaxLength(10000)]
        public string Text { get; set; }
        
        [Required]
        public virtual User User { get; set; }
        
        [Required]
        public virtual MatchInfoModel MatchInfo { get; set; }
    }
}