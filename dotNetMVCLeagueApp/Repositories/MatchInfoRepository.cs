using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Repositories {
    public class MatchInfoRepository : EfCoreRepository<MatchInfoModel, LeagueDbContext> {
        public MatchInfoRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        public IEnumerable<MatchInfoModel> GetLastNMatches(SummonerInfoModel summoner, int n, int start = 0) => LeagueDbContext
            .MatchInfoModels.Where(x => x.SummonerInfoModel.Id == summoner.Id)
            .OrderBy(x => x.PlayTime)
            .Skip(start)
            .Take(n);
    }
}