using System;

namespace dotNetMVCLeagueApp.Utils.Exceptions {
    public class AssetException : Exception {
        public AssetException(string error) : base(error) { }
    }
}