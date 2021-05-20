using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Data.JsonMappings {
    public class Champions {
        [JsonProperty("data")] public Dictionary<string, ChampionAsset> ChampionDict { get; set; }
    }

    /// <summary>
    /// Obsahuje data pro asset postavy - id, jmeno, cestu ... ziskane z champion.json
    /// </summary>
    public class ChampionAsset{
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// Jmeno postavy
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("key")] public string Key { get; set; }

        public string RelativeAssetPath { get; set; }

        public static ChampionAsset Empty(string emptyAsset) => new() {
            RelativeAssetPath = emptyAsset,
            Name = "N/A",
            Key = "N/A"
        };

    }
}