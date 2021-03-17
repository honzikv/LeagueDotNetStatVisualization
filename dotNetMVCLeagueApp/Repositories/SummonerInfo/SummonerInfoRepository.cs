using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Models;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Repositories.SummonerInfo {
    public class SummonerInfoRepository : EfCoreRepository<SummonerInfoModel, LeagueDbContext> {
        public SummonerInfoRepository(LeagueDbContext leagueLeagueDbLeagueDbContext) : base(
            leagueLeagueDbLeagueDbContext) { }

        public async Task<SummonerInfoModel> GetSummonerByUsernameAndRegion(string username, Region region) =>
            await LeagueDbContext.SummonerInfoModels.FirstOrDefaultAsync(summonerInfo =>
                summonerInfo.Name == username && summonerInfo.Region == region.Key);
    }
}