using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    /// <summary>
    ///     M : N tabulka pro propojeni summoner info a match info
    /// </summary>
    public class MatchInfoSummonerInfo {
        public int SummonerInfoModelId { get; set; }

        public long MatchInfoModelId { get; set; }

        public virtual SummonerInfoModel SummonerInfo { get; set; }

        public virtual MatchInfoModel MatchInfo { get; set; }
    }
}