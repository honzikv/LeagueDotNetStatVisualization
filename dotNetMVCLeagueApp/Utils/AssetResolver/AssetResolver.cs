using System;
using System.Collections.Generic;
using System.IO;
using dotNetMVCLeagueApp.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dotNetMVCLeagueApp.Utils.AssetResolver {
    /// <summary>
    /// Trida, ktera slouzi k ziskani cest k souborum jako jsou ikonky, postavy, predmety apod.
    /// </summary>
    public class AssetResolver {
        private readonly AssetResolverConfig config;

        private readonly Dictionary<int, string> champions = new();
        private readonly Dictionary<int, string> summonerSpells = new();

        private const string ErrorFormatGeneric = "Error, json file has incorrect format";

        public AssetResolver(AssetResolverConfig config, string championsFilePath) {
            this.config = config;

            MapChampions(championsFilePath);
        }


        /// <summary>
        /// Ziska parent.childPropertyName jako JObject nebo vyhodi exception, pokud neexistuje / neni JObject
        /// </summary>
        /// <param name="parent">Rodicovsky JObject</param>
        /// <param name="childPropertyName">Jmeno property</param>
        private static JObject GetChildJObjectOrThrowException(JObject parent, string childPropertyName) {
            var dataProperty = parent.Property(childPropertyName);
            if (dataProperty is null || dataProperty.Value.Type != JTokenType.Object) {
                throw new AssetException(ErrorFormatGeneric);
            }

            return (JObject) dataProperty.Value;
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
                throw new AssetException("Error, champions.json contains champion with non-numeric id");
            }
            return (key, (string) propValue.Value);
        }

        /// <summary>
        /// Namapuje postavy podle jejich id s cestou k jejich ikone podle config objektu.
        /// </summary>
        /// <param name="championsJsonFilePath">Cesta k champion.json souboru s postavami</param>
        /// <exception cref="AssetException">Chyba, pokud nelze champion.json parsovat</exception>
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

        private void MapSummonerSpells(string summonerSpellsFilePath) {
            var summonersJson = JObject.Parse(File.ReadAllText(summonerSpellsFilePath));

            // Ziskame json.data ze ktereho muzeme precist vsechny summoner spells
            // Z nejakeho duvodu nejsou v listu, takze iterujeme pres properties JSON objektu
            var summonersJObject = GetChildJObjectOrThrowException(summonersJson, "data");
            foreach (var summonerProperty in summonersJObject.Properties()) {
                // Ziskame jmeno summoneru a id
                var (nameProp, idProp) =
                    GetKeyValueJPropertiesOrThrowException(summonerProperty, "key", "name");

                if (nameProp.Value.Type != JTokenType.String || idProp.Value.Type != JTokenType.String) {
                    throw new AssetException("Error, summoner json has incorrect format");
                }

                var (id, name) = GetKeyValueForDictionary(idProp, nameProp);
                summonerSpells[id] = Path.Combine(config.AssetPath, config.SummonerSpellsFolderName, $"{name}.png");
            }
        }

        private string GetEmptyAsset => Path.Combine(config.AssetPath, $"{config.NotFoundIconId}.png");

        public string GetChampion(int id) => champions.ContainsKey(id)
            ? Path.Combine(config.AssetPath, config.ChampionsFolderName, $"{champions[id]}.png")
            : GetEmptyAsset;

        public string GetItem(int id) => id == default
            ? GetEmptyAsset
            : Path.Combine(config.AssetPath, config.ItemsFolderName, $"{id}.png");

        public string GetProfileIcon(int id) => id == default
            ? GetEmptyAsset
            : Path.Combine(config.AssetPath, config.ProfileIconsFolderName, $"{id}.png");

    }
}