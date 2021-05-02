using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Data.JsonMappings {

    public class RuneTree {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("icon")]
        public string Icon { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("slots")]
        public List<Slot> Slots { get; set; }
    }

    public class Slot {
        [JsonProperty("runes")]
        public List<RuneAsset> Runes { get; set; }
    }

    public class RuneAsset {
        
        [JsonProperty("key")]
        public string Id { get; set; }
        
        /// <summary>
        /// Chceme vzdy aby key byl integer (i kdyz ve stringu z nejakeho duvodu) a tady je jeste k tomu id v "id" misto
        /// ostatnich jsonu, tzn prohodime properties id a key
        /// </summary>
        [JsonProperty("id")]
        public string Key { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("icon")]
        public string Icon { get; set; }
        
        /// <summary>
        /// Tento field je pouze pro konzistenci a je stejny jako Icon
        /// </summary>
        public string RelativeAssetPath { get; set; }

        public static RuneAsset Empty(string configEmptyAssetFileName) => new() {
            Id = "N/A",
            Key = "N/A",
            Name = "N/A",
            Icon = configEmptyAssetFileName,
            RelativeAssetPath = configEmptyAssetFileName
        };
    }
}