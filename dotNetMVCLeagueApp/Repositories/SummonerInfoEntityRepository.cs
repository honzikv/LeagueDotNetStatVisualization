using System;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Repositories {
    public class SummonerInfoEntityRepository : EfCoreEntityRepository<SummonerModel, LeagueDbContext> {
        public SummonerInfoEntityRepository(LeagueDbContext leagueLeagueDbLeagueDbContext) : base(
            leagueLeagueDbLeagueDbContext) { }

        public async Task<SummonerModel> GetSummonerByUsernameAndRegion(string username, Region region) {
            return await LeagueDbContext.SummonerInfoModels.FirstOrDefaultAsync(summonerInfo =>
                // Z nejakeho duvodu neslo String.Equals s ignore capitalization
                summonerInfo.Name.ToLower() == username.ToLower() &&
                summonerInfo.Region == region.Key);
        }
    }
}