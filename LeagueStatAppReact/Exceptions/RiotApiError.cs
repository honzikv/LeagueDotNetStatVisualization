using System;

namespace LeagueStatAppReact.Exceptions {
    public class RiotApiError : ApplicationException {
        public RiotApiError(string message) : base(message) { }
    }
}