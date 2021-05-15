using System;

namespace dotNetMVCLeagueApp.Utils.Exceptions {
    /// <summary>
    /// Vyhazuje se, pokud se stane chyba pri komunikaci s Riot API
    /// </summary>
    public class RiotApiException : ApplicationException {
        public RiotApiException(string message) : base(message) { }
    }
}