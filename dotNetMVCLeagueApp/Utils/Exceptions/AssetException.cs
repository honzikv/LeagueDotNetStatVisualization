using System;

namespace dotNetMVCLeagueApp.Utils.Exceptions {
    
    /// <summary>
    /// Chyba pri manipulaci s assety pomoci AssetRepository
    /// </summary>
    public class AssetException : Exception {
        public AssetException(string error) : base(error) { }
    }
}