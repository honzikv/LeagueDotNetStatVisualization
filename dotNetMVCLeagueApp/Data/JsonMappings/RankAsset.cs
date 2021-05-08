using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Data.JsonMappings {
    public class RankAsset {
        /// <summary>
        /// Tier, ktery slouzi jako klic
        /// </summary>
        [JsonProperty("Tier")]
        public string Tier { get; set; }

        /// <summary>
        /// Jmeno souboru bez koncovky
        /// </summary>
        [JsonProperty("FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Jmeno, ktere se zobrazi na frontendu
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }
        
        public string RelativeAssetPath { get; set; }

        public static RankAsset Empty(string configEmptyAssetFileName) => new() {
            Name = "N/A",
            Tier = "N/A",
            FileName = configEmptyAssetFileName
        };
    }
}