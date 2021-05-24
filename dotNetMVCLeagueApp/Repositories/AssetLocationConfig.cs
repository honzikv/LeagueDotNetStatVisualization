namespace dotNetMVCLeagueApp.Repositories {
    /// <summary>
    /// Jednoduchy objekt, ktery v sobe uklada cesty pro dane soubory.
    /// Pro strukturu predpokladame, ze dane assety jsou ulozene v jedne slozce, ktera obsahuje dalsi slozky
    /// tzn: assets -> [Champions[], Icons[]] ...
    ///
    /// Soubory museji byt identicke se soubory z https://developer.riotgames.com/docs/lol#data-dragon_data-assets
    ///
    /// Navic jsem jeste vytvoril JSON soubor pro obrazky ranku (rank.json)
    ///
    /// Pozn. nelze nastavit cestu pro runy, kde je brana jako vychozi perk-images/..., protoze je struktura
    /// run relativne slozita. Soubory ve slozkach se museji jmenovat stejne jako assety ziskane z Data Dragon
    /// </summary>
    public class AssetLocationConfig {

        /// <summary>
        /// Jmeno slozky s ikonami postav
        /// </summary>
        public string ChampionsFolderName { get; init; }
        
        /// <summary>
        /// Jmeno slozky s ikonami predmetu
        /// </summary>
        public string ItemsFolderName { get; init; }
        
        /// <summary>
        /// Jmeno slozky s ikonami pro hodnocene hry
        /// </summary>
        public string RankedIconsFolderName { get; init; }
        
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