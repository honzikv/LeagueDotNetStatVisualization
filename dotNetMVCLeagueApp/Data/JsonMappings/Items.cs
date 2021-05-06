using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Data.JsonMappings {
    public class Items {
        [JsonProperty("data")] public Dictionary<string, ItemAsset> ItemDict { get; set; }
    }

    public class ItemAsset : MemberwiseCloneable<ItemAsset> {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("plaintext")] public string PlainText { get; set; }

        private Gold gold;

        /// <summary>
        /// Objekt s informacemi o zlatu, autogetter a autosetter jsou vypnute pouze pro jistotu,
        /// aby neslo pri ziskani objektu data zmenit, coz by pomoci MemberWiseCloneable bylo porad mozne
        /// </summary>
        [JsonProperty("gold")]
        public Gold Gold {
            get => gold;
            set => gold ??= value;
        }

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

        public Gold() { }

        public Gold(Gold gold) {
            this.Total = gold.Total;
            this.Sell = gold.Sell;
        }
    }
}