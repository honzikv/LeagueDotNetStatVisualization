using System;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Repositories {
    public class SummonerInfoEntityRepository : EfCoreEntityRepository<SummonerInfoModel, LeagueDbContext> {
        public SummonerInfoEntityRepository(LeagueDbContext leagueLeagueDbLeagueDbContext) : base(
            leagueLeagueDbLeagueDbContext) { }

        public async Task<SummonerInfoModel> GetSummonerByUsernameAndRegion(string username, Region region) {
            return await LeagueDbContext.SummonerInfoModels.FirstOrDefaultAsync(summonerInfo =>
                summonerInfo.Name == username && summonerInfo.Region == region.Key);
        }
    }
}