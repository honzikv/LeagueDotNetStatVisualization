using System.Threading.Tasks;
using LeagueStatAppReact.Data;
using LeagueStatAppReact.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille.Enums;

namespace LeagueStatAppReact.Repositories {
    public class SummonerInfoRepository : EfCoreRepository<SummonerInfoModel, LeagueDbContext> {
        public SummonerInfoRepository(LeagueDbContext leagueLeagueDbLeagueDbContext) : base(
            leagueLeagueDbLeagueDbContext) { }

        public async Task<SummonerInfoModel> GetSummonerByUsernameAndRegion(string username, Region region) {
            return await LeagueDbContext.SummonerInfoModels.FirstOrDefaultAsync(summonerInfo =>
                summonerInfo.Name == username && summonerInfo.Region == region.Key);
        }
    }
}