using System.Threading.Tasks;
using dotNetSpLeagueApp.Models;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV4;

namespace dotNetSpLeagueApp.Repositories.SummonerInfo {
    public interface ISummonerInfoRepository {

        public Task<SummonerInfoModel> GetSummonerInfoAsync(string summonerName, Region region);

        public Task<SummonerInfoModel> GetSummonerInfoFromDbAsync(string summonerName, Region region);

        public Task<SummonerInfoModel> AddSummonerInfoModelToDbAsync(Summoner summoner, Region region);
    }
}