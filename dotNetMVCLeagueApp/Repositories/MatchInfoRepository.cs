using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    public class MatchInfoRepository : EfCoreRepository<MatchInfoModel, LeagueDbContext> {
        public MatchInfoRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }


        /// <summary>
        /// Creates a link in join table MatchInfoSummoner info between specified summoner and match
        /// </summary>
        /// <param name="matchInfoSummonerInfo"></param>
        /// <returns></returns>
        public async Task<MatchInfoSummonerInfo> LinkMatchInfoToSummonerInfo(
            MatchInfoSummonerInfo matchInfoSummonerInfo) {
            LeagueDbContext.MatchInfoSummonerInfos.Add(matchInfoSummonerInfo);
            await LeagueDbContext.SaveChangesAsync();
            return matchInfoSummonerInfo;
        }

        public async Task<MatchInfoSummonerInfo> FindMatchInfoSummonerInfo(long matchInfoId, int summonerInfoId) =>
            await LeagueDbContext.MatchInfoSummonerInfos.Where(matchSummoner =>
                matchSummoner.MatchInfoModelId == matchInfoId &&
                matchSummoner.SummonerInfoModelId == summonerInfoId).FirstOrDefaultAsync();

        /// <summary>
        /// Get last N matches descending by date played for a specific summoner
        /// </summary>
        /// <param name="summoner">Summoner that played in the matches</param>
        /// <param name="n">Number of matches</param>
        /// <param name="start">Start offset</param>
        /// <returns></returns>
        public IEnumerable<MatchInfoModel> GetNLastMatches(SummonerInfoModel summoner, int n, int start = 0) =>
            LeagueDbContext.MatchInfoSummonerInfos
                .Where(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id)
                .OrderByDescending(match => match.MatchInfo.PlayTime)
                .Skip(start)
                .Take(n) // Take maximum of N elements
                .ToList()
                .ConvertAll(matchSummoner => matchSummoner.MatchInfo);

        public async Task<MatchInfoModel> GetMatchInfoWhereId(long id) => await
            LeagueDbContext.MatchInfoModels.SingleOrDefaultAsync(matchInfo => matchInfo.Id == id);
    }
}