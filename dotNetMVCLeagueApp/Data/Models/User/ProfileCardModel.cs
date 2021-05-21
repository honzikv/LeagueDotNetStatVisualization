using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.User {
    /// <summary>
    ///     Entita, ktera reprezentuje karticku k profilu
    ///     Obsahuje referenci na uzivatele (pres nej lze ziskat summoner info) a na zapas
    /// </summary>
    public class ProfileCardModel {
        public int Id { get; set; }
        
        /// <summary>
        /// Header karticky - nadpis
        /// </summary>
        public string PrimaryText { get; set; }

        /// <summary>
        ///     Text karticky - bud link nebo plain text
        /// </summary>
        public string SecondaryText { get; set; }

        /// <summary>
        ///     Zda-li se jedna o odkaz na social media
        /// </summary>
        public bool SocialMedia { get; set; }

        /// <summary>
        ///     Pozice na profilu
        /// </summary>
        public int Position { get; set; }
        
        /// <summary>
        /// Reference na uzivatele
        /// </summary>
        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}