using System;

namespace dotNetMVCLeagueApp.Exceptions {
    public class RiotApiError : ApplicationException {
        public RiotApiError(string message) : base(message) { }
    }
}