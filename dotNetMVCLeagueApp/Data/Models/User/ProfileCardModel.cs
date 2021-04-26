using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data.Models.User {
    /// <summary>
    ///     Entita, ktera reprezentuje karticku k profilu
    ///     Obsahuje referenci na uzivatele (pres nej lze ziskat summoner info) a na zapas
    /// </summary>
    public class ProfileCardModel {
        public int Id { get; set; }

        /// <summary>
        ///     Text karticky - bud link nebo UTF-8 plain text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Zda-li se jedna o odkaz na social media
        /// </summary>
        public bool SocialMedia { get; set; }

        /// <summary>
        ///     Reference na uzivatele
        /// </summary>
        [Required]
        public virtual UserModel UserModel { get; set; }

        /// <summary>
        ///     Pozice na profilu
        /// </summary>
        public int Position { get; set; }
    }
}