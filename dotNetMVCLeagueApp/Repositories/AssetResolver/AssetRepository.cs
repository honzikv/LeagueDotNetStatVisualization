using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotNetMVCLeagueApp.Data.JsonMappings;
using dotNetMVCLeagueApp.Exceptions;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Repositories.AssetResolver {
    /// <summary>
    /// Objekt, ktery pri startu programu provede parsing souboru
    /// </summary>
    public class AssetRepository {
        /// <summary>
        /// Id postavy -> data o assetu
        /// </summary>
        private readonly Dictionary<int, ChampionAsset> champions = new();

        /// <summary>
        /// Id summoner spellu -> data o assetu
        /// </summary>
        /// <returns></returns>
        private readonly Dictionary<int, SummonerSpellAsset> summonerSpells = new();

        /// <summary>
        /// Id runy -> data o assetu
        /// </summary>
        private readonly Dictionary<int, RuneAsset> runes = new();

        /// <summary>
        /// Id predmetu -> data o assetu
        /// </summary>
        private readonly Dictionary<int, ItemAsset> items = new();

        private readonly Dictionary<string, RankAsset> ranks = new();

        /// <summary>
        /// Config objekt s informacemi o umisteni assetu
        /// </summary>
        private readonly AssetLocationConfig config;

        /// <summary>
        /// Jednoducha interpolace stringu pro vlozeni jmena jsonu kde se stala chyba
        /// </summary>
        /// <param name="fileName">jmeno json souboru</param>
        /// <returns>Chybovou zpravu pro dany json soubor</returns>
        private static string ErrorForJsonFile(string fileName) =>
            $"Error, {fileName}.json file has incorrect format";

        public AssetRepository(AssetLocationConfig config,
            string championsJsonFilePath,
            string summonerSpellsJsonFilePath,
            string runesJsonFilePath,
            string itemsJsonFilePath,
            string rankedIconsJsonFilePath) {
            this.config = config;

            // Inicializace repozitare
            try {
                MapChampions(championsJsonFilePath);
                MapSummonerSpells(summonerSpellsJsonFilePath);
                MapRunes(runesJsonFilePath);
                MapItems(itemsJsonFilePath);
                MapRankedIcons(rankedIconsJsonFilePath);
            }
            catch (Exception ex) {
                if (ex is AssetException) {
                    throw;
                }

                // Jinak hodime generickou zpravu protoze nastal problem s ctenim json souboru
                Console.WriteLine(ex.Message);
                throw new IOException("Error, while reading json files");
            }
        }

        /// <summary>
        /// Genericke mapovani Json objektu ze souboru na C# objekt typu T
        /// </summary>
        /// <param name="jsonFilePath">Cesta k json souboru</param>
        /// <param name="jsonFileNameWithoutExtension">Nazev json souboru bez koncovky (pro log chyby)</param>
        /// <typeparam name="T">Typ C# objektu</typeparam>
        /// <returns>Namapovany C# objekt</returns>
        /// <exception cref="AssetException">Chyba, pokud nelze soubor namapovat</exception>
        private T ParseJsonObject<T>(string jsonFilePath, string jsonFileNameWithoutExtension) {
            var jsonString = File.ReadAllText(jsonFilePath);
            var jsonObj = JsonConvert.DeserializeObject<T>(jsonString);

            if (jsonObj is null) {
                throw new AssetException(ErrorForJsonFile(jsonFileNameWithoutExtension));
            }

            return jsonObj;
        }

        private void MapChampions(string championsJsonFilePath) {
            var jsonObj = ParseJsonObject<Champions>(championsJsonFilePath, "champions");

            // Nyni iterujeme pres C# mapping json objektu a ulozime do repozitare
            foreach (var champion in jsonObj.ChampionDict.Values) {
                var parsed = int.TryParse(champion.Key, out var championId);
                if (!parsed) {
                    throw new AssetException(ErrorForJsonFile("champions"));
                }

                champion.RelativeAssetPath = Path.Combine(config.ChampionsFolderName, $"{champion.Id}.png");
                champions[championId] = champion;
            }
        }

        /// <summary>
        /// Namapuje summoner spelly ze souboru summoner.json na C# objekty
        /// </summary>
        /// <param name="summonerSpellsJsonFilePath">Cesta k json souboru</param>
        /// <exception cref="AssetException">Chyba, pokud neni json validni</exception>
        private void MapSummonerSpells(string summonerSpellsJsonFilePath) {
            var jsonObj = ParseJsonObject<SummonerSpells>(summonerSpellsJsonFilePath, "summoner");

            foreach (var summonerSpell in jsonObj.SpellDict.Values) {
                var parsed = int.TryParse(summonerSpell.Key, out var summonerSpellId);
                if (!parsed) {
                    throw new AssetException(ErrorForJsonFile("summoner"));
                }

                summonerSpell.RelativeAssetPath =
                    Path.Combine(config.SummonerSpellsFolderName, $"{summonerSpell.Id}.png");
                summonerSpells[summonerSpellId] = summonerSpell;
            }
        }

        /// <summary>
        /// Namapuje runy ze souboru runesReforged.json na C# objekty 
        /// </summary>
        /// <param name="runesJsonFilePath">Cesta k json souboru</param>
        /// <exception cref="AssetException">Chyba, pokud neni json validni</exception>
        private void MapRunes(string runesJsonFilePath) {
            var jsonArray = ParseJsonObject<List<RuneTree>>(runesJsonFilePath, "runesReforged");

            // Zde jsou data relativne vnorena, takze musime provest nekolik for cyklu abychom dostali
            // samotne runy. Objekt rune tree budeme zobrazovat take a potrebujeme k nemu stejne informace
            // jako u  runy, takze ho namapujeme na runu a ulozime do slovniku
            foreach (var tree in jsonArray) {
                var runeTree = new RuneAsset { // namapujeme na runu pro ulozeni do slovniku (pro potreby aplikace neni relevantni)
                    Id = tree.Id,
                    Icon = tree.Icon,
                    Name = tree.Name,
                    RelativeAssetPath = tree.Icon
                };

                // Test, zda-li je jmeno int
                var parsed = int.TryParse(runeTree.Id, out var runeTreeId);
                if (!parsed) {
                    throw new AssetException(ErrorForJsonFile("runesReforged"));
                }

                runes[runeTreeId] = runeTree; // ulozime do slovniku

                // Nyni iterujeme pro sloty, kde je seznam run - tzn. SelectMany
                foreach (var rune in tree.Slots.SelectMany(slot => slot.Runes)) {
                    parsed = int.TryParse(rune.Key, out var runeId);
                    if (!parsed) {
                        throw new AssetException(ErrorForJsonFile("runesReforged"));
                    }

                    rune.RelativeAssetPath = rune.Icon;
                    runes[runeId] = rune;
                }
            }
        }

        /// <summary>
        /// Namapuje predmety ze souboru item.json
        /// </summary>
        /// <param name="itemsJsonFilePath">Cesta k item.json</param>
        /// <exception cref="AssetException">Chyba, pokud je item.json nevalidni</exception>
        private void MapItems(string itemsJsonFilePath) {
            var jsonObj = ParseJsonObject<Items>(itemsJsonFilePath, "item");

            foreach (var (key, item) in jsonObj.ItemDict) {
                var parsed = int.TryParse(key, out var itemId);
                if (!parsed) {
                    throw new AssetException(ErrorForJsonFile("item"));
                }

                item.RelativeAssetPath = Path.Combine(config.ItemsFolderName, $"{itemId}.png");
                items[itemId] = item;
            }
        }

        private void MapRankedIcons(string rankedIconsJsonFilePath) {
            var jsonObj = ParseJsonObject<List<RankAsset>>(rankedIconsJsonFilePath, "rank");

            foreach (var rankAsset in jsonObj) {
                ranks[rankAsset.Tier] = rankAsset;
            }
        }

        /// <summary>
        /// Ziska asset objekt pro postavu nebo vrati prazdnou postavu pokud je id nevalidni
        /// </summary>
        /// <param name="championId">Id postavy</param>
        /// <returns>Asset objekt s daty</returns>
        public ChampionAsset GetChampionAsset(int championId) {
            var exists = champions.TryGetValue(championId, out var champion);
            return exists ? champion : ChampionAsset.Empty(config.EmptyAssetFileName);
        }

        /// <summary>
        /// Ziska asset objekt pro summoner spell nebo vrati prazdny, pokud id neexistuje
        /// </summary>
        /// <param name="summonerSpellId"></param>
        /// <returns>SummonerSpell objekt s informacemi o assetu</returns>
        public SummonerSpellAsset GetSummonerSpellAsset(int summonerSpellId) {
            var exists = summonerSpells.TryGetValue(summonerSpellId, out var summonerSpell);
            return exists ? summonerSpell : SummonerSpellAsset.Empty(config.EmptyAssetFileName);
        }

        /// <summary>
        /// Ziska asset objekt pro runu nebo vrati prazdny pri spatnem id
        /// </summary>
        /// <param name="runeId">Id runy</param>
        /// <returns>Rune objekt s informacemi o assetu</returns>
        public RuneAsset GetRuneAsset(int runeId) {
            var exists = runes.TryGetValue(runeId, out var rune);
            return exists ? rune : RuneAsset.Empty(config.EmptyAssetFileName);
        }

        /// <summary>
        /// Ziska asset objekt pro predmet nebo vrati prazdny pri spatnem id
        /// </summary>
        /// <param name="itemId">Id predmetu</param>
        /// <returns>Item objekt s informacemi o assetu</returns>
        public ItemAsset GetItemAsset(int itemId) {
            var exists = items.TryGetValue(itemId, out var item);
            return exists ? item : ItemAsset.Empty(config.EmptyAssetFileName);
        }

        /// <summary>
        /// Ikonky se uz jmenuji podle id, takze staci ziskat z configu cestu a vratit cesta/id.png.
        /// Pokud je predmet default (0 - prazdny) tak vratime cernou ikonku
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Cestu k profilove ikonce</returns>
        public string GetProfileIcon(int id) => id == default
            ? config.EmptyAssetFileName
            : Path.Combine(config.ProfileIconsFolderName, $"{id}.png");

        public RankAsset GetRankedIcon(string tier) {
            var exists = ranks.TryGetValue(tier, out var rankAsset);
            return exists ? rankAsset : RankAsset.Empty(config.EmptyAssetFileName);
        }
    }

}