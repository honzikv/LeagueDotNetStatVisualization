using System;
using System.Collections.Generic;
using System.IO;
using dotNetMVCLeagueApp.Exceptions;
using Newtonsoft.Json.Linq;

namespace dotNetMVCLeagueApp.Services.AssetResolver {
    /// <summary>
    /// Trida, ktera slouzi k ziskani cest k souborum jako jsou ikonky, postavy, predmety apod.
    /// 
    /// </summary>
    public class AssetResolverService {
        private readonly AssetResolverConfig config;

        /// <summary>
        /// Obsahuje mapping id na cestu k ikonce pro danou postavu
        /// </summary>
        private readonly Dictionary<int, string> champions = new();

        /// <summary>
        /// Obsahuje mapping id na cestu k ikonce pro dany summoner spell
        /// </summary>
        private readonly Dictionary<int, string> summonerSpells = new();

        /// <summary>
        /// Obsahuje mapping id na cestu k ikonce pro runu
        /// </summary>
        private readonly Dictionary<int, string> runes = new();

        /// <summary>
        /// Vychozi genericka zprava, pokud je nejaky z json souboru, ze kterych se id ziskavaji
        /// poskozeny
        /// </summary>
        private const string ErrorFormatGeneric = "Error, json file has incorrect format";

        public AssetResolverService(AssetResolverConfig config,
            string championsJsonFilePath,
            string summonerSpellsJsonFilePath,
            string runesJsonFilePath) {
            this.config = config;


            try {
                MapChampions(championsJsonFilePath);
                MapSummonerSpells(summonerSpellsJsonFilePath);
                MapRunes(runesJsonFilePath);
            }
            catch (Exception ex) {
                // Vyhodime jakoukoliv chybu jako AssetException
                if (ex is AssetException) {
                    throw;
                }

                throw new AssetException(ex.Message);
            }
        }


        /// <summary>
        /// Ziska parent.childPropertyName jako JObject nebo vyhodi exception, pokud neexistuje / neni JObject
        /// </summary>
        /// <param name="parent">Rodicovsky JObject</param>
        /// <param name="childPropertyName">Jmeno property</param>
        private static JObject GetChildJObjectOrThrowException(JObject parent, string childPropertyName) {
            var childProperty = parent.Property(childPropertyName);
            if (childProperty is null || childProperty.Value.Type != JTokenType.Object) {
                throw new AssetException(ErrorFormatGeneric);
            }

            return (JObject) childProperty.Value;
        }

        private static JArray GetChildJArrayOrThrowException(JObject parent, string childPropertyName) {
            var childProperty = parent.Property(childPropertyName);
            if (childProperty is null || childProperty.Value.Type != JTokenType.Array) {
                throw new AssetException(ErrorFormatGeneric);
            }

            return (JArray) childProperty.Value;
        }

        /// <summary>
        /// Override pro JToken
        /// </summary>
        /// <param name="token"></param>
        /// <param name="childPropertyName"></param>
        /// <returns></returns>
        private static JArray GetChildJArrayOrThrowException(JToken token, string childPropertyName) {
            if (token.Type != JTokenType.Object) {
                throw new AssetException(ErrorFormatGeneric);
            }

            return GetChildJArrayOrThrowException((JObject) token, childPropertyName);
        }

        /// <summary>
        /// Ziska key-value z dane property, ktera musi byt JObject
        /// </summary>
        /// <param name="property">JProperty, ze ktere klic a hodnotu ziskavame</param>
        /// <param name="keyProperty">jmeno klice</param>
        /// <param name="valueProperty">jmeno hodnoty</param>
        /// <returns>Par klic a  hodnota</returns>
        /// <exception cref="AssetException">Vyhodi exception,</exception>
        private static (JProperty, JProperty) GetKeyValueJPropertiesOrThrowException(JProperty property,
            string keyProperty, string valueProperty) {
            if (property.Value.Type != JTokenType.Object) {
                throw new AssetException(ErrorFormatGeneric);
            }

            // Jinak je JObject a pretypujeme
            var jObject = (JObject) property.Value;
            return GetKeyValueJPropertiesOrThrowException(jObject, keyProperty, valueProperty);
        }

        private static (JProperty, JProperty) GetKeyValueJPropertiesOrThrowException(JObject jObject,
            string keyProperty, string valueProperty) {
            // Ziskame klic a hodnotu
            var key = jObject.Property(keyProperty);
            var value = jObject.Property(valueProperty);

            // Zjistime zda-li jsou null a pokud ne tak vratime, jinak vyhodime exception (to by se melo stat
            // pouze v pripade ze je json soubor poskozeny)
            if (key is null || value is null) {
                throw new AssetException(ErrorFormatGeneric);
            }

            return (key, value);
        }

        private static (int, string) GetKeyValueForDictionary(JProperty propKey, JProperty propValue) {
            var isKeyNumeric = int.TryParse((string) propKey.Value, out var key);
            if (!isKeyNumeric) {
                throw new AssetException("Error, key property is not a number");
            }

            return (key, (string) propValue.Value);
        }

        /// <summary>
        /// Namapuje postavy podle jejich id s cestou k jejich ikone podle config objektu.
        /// Muze vyhodit i exception pro Files.ReadAllText
        /// </summary>
        /// <param name="championsJsonFilePath">Cesta k champion.json souboru s postavami</param>
        /// <exception cref="AssetException">Vyhodi exception pokud je json soubor nevalidni</exception>
        private void MapChampions(string championsJsonFilePath) {
            var championJson = JObject.Parse(File.ReadAllText(championsJsonFilePath));

            // Z nejakeho duvodu nejsou data array, ale misto toho jsou postavy jako property,
            // Tzn namapujeme kazdou property do dictionary
            var championsJObject = GetChildJObjectOrThrowException(championJson, "data");
            foreach (var championProperty in championsJObject.Properties()) {
                // Ziskame jmeno postavy a id
                var (idProp, nameProp) =
                    GetKeyValueJPropertiesOrThrowException(championProperty, "key", "name");

                if (idProp.Value.Type != JTokenType.String || nameProp.Value.Type != JTokenType.String) {
                    throw new AssetException("Error, champion json has incorrect format");
                }

                var (id, name) = GetKeyValueForDictionary(idProp, nameProp);
                champions[id] = Path.Combine(config.AssetPath, config.ChampionsFolderName, $"{name}.png");
            }
        }

        /// <summary>
        /// Namapuje id summoner spellu na cestu k assetu
        /// </summary>
        /// <param name="summonerSpellsJsonFilePath"></param>
        /// <exception cref="AssetException">Vyhodi exception pokud je json soubor nevalidni</exception>
        private void MapSummonerSpells(string summonerSpellsJsonFilePath) {
            var summonersJson = JObject.Parse(File.ReadAllText(summonerSpellsJsonFilePath));

            // Ziskame json.data ze ktereho muzeme precist vsechny summoner spells
            // Z nejakeho duvodu nejsou v listu, takze iterujeme pres properties JSON objektu
            var summonersJObject = GetChildJObjectOrThrowException(summonersJson, "data");
            foreach (var summonerProperty in summonersJObject.Properties()) {
                // Ziskame jmeno summoneru a id
                var (idProp, nameProp) =
                    GetKeyValueJPropertiesOrThrowException(summonerProperty, "key", "name");

                if (nameProp.Value.Type != JTokenType.String || idProp.Value.Type != JTokenType.String) {
                    throw new AssetException("Error, summoner json has incorrect format");
                }

                var (id, name) = GetKeyValueForDictionary(idProp, nameProp);
                summonerSpells[id] = Path.Combine(config.AssetPath, config.SummonerSpellsFolderName, $"{name}.png");
            }
        }

        /// <summary>
        /// Namapuje id run na cestu k ikonam
        /// </summary>
        /// <param name="runesJsonFilePath">Json soubor s informacemi o id</param>
        /// <exception cref="AssetException">Vyhodi exception pokud je json soubor nevalidni</exception>
        private void MapRunes(string runesJsonFilePath) {
            var runesJson = JArray.Parse(File.ReadAllText(runesJsonFilePath));

            // Ve hre jsou runy umistene do "trees" - stromu, ze kterych si hrac runy vybira, tech je nekolik
            // a kazda runa ma navic pozici
            // Tzn. - budeme iterovat pres pole a pro kazdy strom namapujeme vsechny runy do slovniku
            foreach (var runeTree in runesJson.Children()) {
                var slots = GetChildJArrayOrThrowException(runeTree, "slots");
                foreach (var slot in slots.Children()) {
                    // Nyni konecne dostaneme array, kde jsou jednotlive runy pro danou pozici
                    var runesJArray = GetChildJArrayOrThrowException(slot, "runes");
                    foreach (var runeToken in runesJArray.Children()) {
                        var rune = runeToken.Type == JTokenType.Object
                            ? (JObject) runeToken
                            : throw new AssetException("Error, runes json file has incorrect format");

                        // Ziskame id a cestu k souboru
                        var (keyProp, pathProp) =
                            GetKeyValueJPropertiesOrThrowException(rune, "id", "icon");

                        var key = keyProp.Value.Type == JTokenType.Integer
                            ? (int) keyProp.Value
                            : throw new AssetException("Error, runes json file has incorrect format");

                        var relativePath = pathProp.Value.Type == JTokenType.String
                            ? (string) pathProp.Value
                            : throw new AssetException("Error, runes json file has incorrect format");

                        // Pokud by bylo null, pak take vyhodime exception (nicmene to nikdy nenastane pri spravnem
                        // formatu souboru
                        runes[key] = Path.Combine(config.AssetPath,
                            relativePath ?? throw new AssetException("Error, runes json file has incorrect format"));
                    }
                }
            }
        }

        /// <summary>
        /// Vrati prazdny asset - cerny ctverec
        /// </summary>
        private string GetEmptyAsset => Path.Combine(config.AssetPath, config.EmptyAssetFileName);

        /// <summary>
        /// Vrati predmet z daneho dictionary nebo prazdny asset
        /// </summary>
        /// <param name="dictionary">dany dictionary, ze ktereho hledame</param>
        /// <param name="id">id predmetu</param>
        /// <returns>predmet z kolekce nebo cestu k "prazdnemu" assetu</returns>
        private string GetItemOrEmptyAsset(IDictionary<int, string> dictionary, int id) {
            var exists = dictionary.TryGetValue(id, out var path);
            return exists ? path : GetEmptyAsset;
        }

        /// <summary>
        /// Ziska cestu k postave (nebo cerny ctverec, pokud id je neplatne)
        /// </summary>
        /// <param name="id">id postavy</param>
        /// <returns>Cestu k ikone postavy</returns>
        public string GetChampion(int id) => GetItemOrEmptyAsset(this.champions, id);

        /// <summary>
        /// Ziska cestu k summoner spellu (nebo cerny ctverec, pokud je id neplatne)
        /// </summary>
        /// <param name="id">id summoner spellu</param>
        /// <returns>Cestu k ikone summoner spellu</returns>
        public string GetSummonerSpell(int id) => GetItemOrEmptyAsset(this.summonerSpells, id);


        /// <summary>
        /// Ziska runu (nebo cerny ctverec, pokud neexistuje)
        /// </summary>
        /// <param name="id">id runy</param>
        /// <returns>Cestu k ikonce runy</returns>
        public string GetRune(int id) => GetItemOrEmptyAsset(this.runes, id);

        /// <summary>
        /// Predmety se uz jmenuji podle id, takze staci ziskat z configu cestu a vratit cesta/id.png.
        /// Pokud je predmet default (0 - prazdny) tak vratime cernou ikonku
        /// </summary>
        /// <param name="id">Id predmetu ziskane z api</param>
        /// <returns>Cestu k ikonce predmetu</returns>
        public string GetItem(int id) => id == default
            ? GetEmptyAsset
            : Path.Combine(config.AssetPath, config.ItemsFolderName, $"{id}.png");

        /// <summary>
        /// Ikonky se uz jmenuji podle id, takze staci ziskat z configu cestu a vratit cesta/id.png.
        /// Pokud je predmet default (0 - prazdny) tak vratime cernou ikonku
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Cestu k profilove ikonce</returns>
        public string GetProfileIcon(int id) => id == default
            ? GetEmptyAsset
            : Path.Combine(config.AssetPath, config.ProfileIconsFolderName, $"{id}.png");
    }
}