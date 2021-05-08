using System;

namespace dotNetMVCLeagueApp.Exceptions {
    /// <summary>
    /// Typ vyjimky, kdy musime presmerovat uzivatele na domovskou stranku, protoze se stala chyba,
    /// kterou nelze opravit
    /// </summary>
    public class RedirectToHomePageException : Exception {
        public RedirectToHomePageException(string message) : base(message) { }
    }
}