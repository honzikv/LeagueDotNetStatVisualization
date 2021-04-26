namespace dotNetMVCLeagueApp.Data.Models.SummonerPage {
    public class QueueInfoModel {
        public int Id { get; set; }

        /// <summary>
        ///     Typ queue - ranked, solo ...
        /// </summary>
        public string QueueType { get; set; }

        /// <summary>
        ///     i.e. Diamond, Gold, Platinum ...
        /// </summary>
        public string Tier { get; set; }

        /// <summary>
        ///     i.e. II, III, IV ...
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        ///     Pocet LP
        /// </summary>
        public int LeaguePoints { get; set; }

        /// <summary>
        ///     Pocet vyher
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        ///     Pocet proher
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        ///     Reference na summoner info
        /// </summary>
        public virtual SummonerInfoModel SummonerInfo { get; set; }
    }
}