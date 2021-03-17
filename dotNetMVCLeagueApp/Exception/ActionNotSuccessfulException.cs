using System;

namespace dotNetMVCLeagueApp.Exception {
    /// <summary>
    /// Simple exception that is used when an operation did not succeed
    /// </summary>
    public class ActionNotSuccessfulException : ApplicationException {
        public ActionNotSuccessfulException(string message) : base(message) { }
    }
}