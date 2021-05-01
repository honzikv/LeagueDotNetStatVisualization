using System.Diagnostics.CodeAnalysis;

namespace dotNetMVCLeagueApp.Services.AssetResolver {
    /// <summary>
    /// Jednoduchy objekt, ktery v sobe uklada cesty pro dane soubory.
    /// Pro strukturu predpokladame, ze dane assety jsou ulozene v jedne slozce, ktera obsahuje dalsi slozky
    /// tzn: assets -> [Champions[], Icons[]] ...
    ///
    /// Soubory museji byt identicke se soubory z https://developer.riotgames.com/docs/lol#data-dragon_data-assets
    ///
    /// Pozn. nelze nastavit cestu pro runy, kde je brana jako vychozi perk-images/..., protoze je struktura
    /// run relativne slozita. Soubory ve slozkach se museji jmenovat stejne jako assety ziskane z Data Dragon
    /// </summary>
    public class AssetResolverConfig {
        
        /// <summary>
        /// Cesta k assetum (absolutni)
        /// </summary>
        public string AssetPath { get; init; }
        
        /// <summary>
        /// Jmeno slozky s ikonami postav
        /// </summary>
        public string ChampionsFolderName { get; init; }
        
        public string ItemsFolderName { get; init; }
        
        /// <summary>
        /// Jmeno slozky pro summoner spells
        /// </summary>
        public string SummonerSpellsFolderName { get; init; }
        
        /// <summary>
        /// Jmeno slozky pro ikonky uzivatele
        /// </summary>
        public string ProfileIconsFolderName { get; init; }

        /// <summary>
        /// Ikona pro nenalezeny asset
        /// </summary>
        public string EmptyAssetFileName { get; init; }
    }
}