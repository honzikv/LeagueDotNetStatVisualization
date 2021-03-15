using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Models;
using dotNetSpLeagueApp.Repositories.SummonerInfo;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV4;

namespace dotNetMVCLeagueApp.Repositories.SummonerInfo {
    public class SummonerInfoRepository : EfCoreRepository<SummonerInfoModel, LeagueDbContext> {

        public SummonerInfoRepository(LeagueDbContext leagueDbDbContext) : base(leagueDbDbContext) {
        }

        public async Task<SummonerInfoModel> GetSummonerInfoAsync(string summonerName, Region region) {
            throw new System.NotImplementedException();
        }

        public async Task<SummonerInfoModel> GetSummonerInfoFromDbAsync(string summonerName, Region region) {
            throw new System.NotImplementedException();
        }

        public async Task<SummonerInfoModel> AddSummonerInfoModelToDbAsync(Summoner summoner, Region region) {
            throw new System.NotImplementedException();
        }
    }
}