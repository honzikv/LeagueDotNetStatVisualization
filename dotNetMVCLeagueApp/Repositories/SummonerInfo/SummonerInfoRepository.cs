using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetSpLeagueApp.Models;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV4;

namespace dotNetSpLeagueApp.Repositories.SummonerInfo {
    public class SummonerInfoRepository : ISummonerInfoRepository {

        private ApplicationDbContext applicationDbContext;

        public SummonerInfoRepository(ApplicationDbContext applicationDbContext) {
            this.applicationDbContext = applicationDbContext;
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