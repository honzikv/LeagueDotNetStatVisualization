using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    /// <summary>
    ///     M : N tabulka pro propojeni summoner info a match info
    /// </summary>
    public class MatchToSummonerModel {
        public int SummonerInfoModelId { get; set; }

        public long MatchInfoModelId { get; set; }

        public virtual SummonerModel Summoner { get; set; }

        public virtual MatchModel Match { get; set; }
    }
}