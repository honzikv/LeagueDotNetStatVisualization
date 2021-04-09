using System.ComponentModel.DataAnnotations.Schema;
using Castle.Components.DictionaryAdapter;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.Models.Match {
    /// <summary>
    /// M : N table for connecting matches and summoners
    /// </summary>
    public class MatchInfoSummonerInfo {
        
        public int SummonerInfoModelId { get; set; }
        
        public long MatchInfoModelId { get; set; }
        
        public SummonerInfoModel SummonerInfo { get; set; }
        
        public MatchInfoModel MatchInfo { get; set; }
    }
}