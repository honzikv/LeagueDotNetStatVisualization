using System;

namespace dotNetMVCLeagueApp.Exceptions {
    /// <summary>
    /// Vyhazuje se, pokud se stane chyba pri komunikaci s Riot API
    /// </summary>
    public class RiotApiError : ApplicationException {
        public RiotApiError(string message) : base(message) { }
    }
}