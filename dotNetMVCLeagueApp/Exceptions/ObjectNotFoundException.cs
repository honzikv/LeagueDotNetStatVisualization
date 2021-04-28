using System;

namespace dotNetMVCLeagueApp.Exceptions {
    public class ObjectNotFoundException : ApplicationException {
        public ObjectNotFoundException(string message) : base(message) {
            
        }
    }
}