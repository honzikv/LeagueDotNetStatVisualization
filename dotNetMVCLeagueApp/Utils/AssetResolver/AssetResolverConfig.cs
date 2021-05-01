using System.Diagnostics.CodeAnalysis;

namespace dotNetMVCLeagueApp.Utils.AssetResolver {
    /// <summary>
    /// Jednoduchy objekt, ktery v sobe uklada cesty pro dane soubory.
    /// Pro strukturu predpokladame, ze dane assety jsou ulozene v jedne slozce, ktera obsahuje dalsi slozky
    /// tzn: assets -> [Champions[], Icons[]] ...
    /// </summary>
    public class AssetResolverConfig {
        
        /// <summary>
        /// Cesta k assetum (absolutni)
        /// </summary>
        public string AssetPath { get; }
        
        /// <summary>
        /// Jmeno slozky s ikonami postav
        /// </summary>
        public string ChampionsFolderName { get; }
        
        public string ItemsFolderName { get; }
        
        /// <summary>
        /// Jmeno slozky pro summoner spells
        /// </summary>
        public string SummonerSpellsFolderName { get; }
        
        /// <summary>
        /// Jmeno slozky pro ikonky uzivatele
        /// </summary>
        public string ProfileIconsFolderName { get; }
        
        /// <summary>
        /// Jmeno slozky pro runy
        /// </summary>
        public string RunesFolderName { get; }
        
        /// <summary>
        /// Ikona pro nenalezeny asset
        /// </summary>
        public string NotFoundIconId { get; }
    }
}