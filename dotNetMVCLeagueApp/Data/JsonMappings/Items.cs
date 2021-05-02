using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Data.JsonMappings {
    public class Items {
        [JsonProperty("data")] public Dictionary<string, ItemAsset> ItemDict { get; set; }
    }

    public class ItemAsset {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("plaintext")] public string PlainText { get; set; }

        [JsonProperty("gold")] public Gold Gold { get; set; }

        public string RelativeAssetPath { get; set; }

        public static ItemAsset Empty(string emptyAsset) => new() {
            Name = "N/A",
            Description = "N/A",
            PlainText = "",
            Gold = new() {Total = -1, Sell = -1},
            RelativeAssetPath = emptyAsset
        };
    }

    public class Gold {
        [JsonProperty("total")] public int Total { get; set; }

        [JsonProperty("sell")] public int Sell { get; set; }
    }
}