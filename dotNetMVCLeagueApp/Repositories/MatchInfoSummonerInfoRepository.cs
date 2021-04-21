using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.Match;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    public class MatchInfoSummonerInfoRepository : EfCoreEntityRepository<MatchInfoSummonerInfo, LeagueDbContext> {
        public MatchInfoSummonerInfoRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        /// <summary>
        /// Zjisti, zda-li je nejaky zaznam mezi danym summonerem a zapasem
        /// </summary>
        /// <param name="matchInfoId">Id zapasu</param>
        /// <param name="summonerInfoId">Id uzivatele v zapasu</param>
        /// <returns>True pokud ano, jinak false</returns>
        public async Task<bool> AnyJoinBetweenMatchSummoner(long matchInfoId, int summonerInfoId) =>
            await LeagueDbContext.MatchInfoSummonerInfos.AnyAsync(matchSummoner =>
                matchSummoner.MatchInfoModelId == matchInfoId &&
                matchSummoner.SummonerInfoModelId == summonerInfoId);
    }
}