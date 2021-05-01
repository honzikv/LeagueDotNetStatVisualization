using System;

namespace dotNetMVCLeagueApp.Exceptions {
    public class AssetException : Exception {
        public AssetException(string error) : base(error) { }
    }
}