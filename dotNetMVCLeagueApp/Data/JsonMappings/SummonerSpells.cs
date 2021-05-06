using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Data.JsonMappings {
    public class SummonerSpells {
        
        [JsonProperty("data")]
        public Dictionary<string, SummonerSpellAsset> SpellDict { get; set; }
    }

    public class SummonerSpellAsset : MemberwiseCloneable<SummonerSpellAsset> {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("key")]
        public string Key { get; set; }
        
        public string RelativeAssetPath { get; set; }

        public static SummonerSpellAsset Empty(string configEmptyAssetFileName) => new() {
            Id = "N/A",
            Name = "N/A",
            Description = "N/A",
            Key = "N/A",
            RelativeAssetPath = configEmptyAssetFileName
        };
    }
}