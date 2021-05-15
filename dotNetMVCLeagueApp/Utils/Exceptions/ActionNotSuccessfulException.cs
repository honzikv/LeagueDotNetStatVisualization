using System;

namespace dotNetMVCLeagueApp.Utils.Exceptions {
    /// <summary>
    /// Vyhazuje se vetsinou v Service kdyz dojde k nejake chybe
    /// </summary>
    public class ActionNotSuccessfulException : ApplicationException {
        public ActionNotSuccessfulException(string message) : base(message) { }
    }
}