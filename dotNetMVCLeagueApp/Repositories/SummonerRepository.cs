using System;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Repositories {
    public class SummonerRepository : EfCoreEntityRepository<SummonerModel, LeagueDbContext> {
        public SummonerRepository(LeagueDbContext leagueLeagueDbLeagueDbContext) : base(
            leagueLeagueDbLeagueDbContext) { }

        /// <summary>
        /// Ziska Summoner ucet podle uzivatelskeho jmena a regionu
        /// </summary>
        /// <param name="username">Uzivatelske jmeno</param>
        /// <param name="region">Server / region - EUW, EUNE, NA</param>
        /// <returns></returns>
        public async Task<SummonerModel> GetSummonerByUsernameAndRegion(string username, Region region) {
            return await LeagueDbContext.SummonerModels.FirstOrDefaultAsync(summonerInfo =>
                // Z nejakeho duvodu neslo String.Equals s ignore capitalization
                summonerInfo.Name.ToLower() == username.ToLower() &&
                summonerInfo.Region == region.Key);
        }
    }
}