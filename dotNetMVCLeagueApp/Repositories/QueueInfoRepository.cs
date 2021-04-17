using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Repositories {
    public class QueueInfoRepository : EfCoreEntityRepository<QueueInfoModel, LeagueDbContext> {
        public QueueInfoRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }
    }
}