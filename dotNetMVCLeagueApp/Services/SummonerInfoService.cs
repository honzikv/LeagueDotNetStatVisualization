using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Models;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Repositories.SummonerInfo;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// This service provides information about specific summoner either by calling a RiotApi or getting data from
    /// database
    /// </summary>
    public class SummonerInfoService {
        private readonly SummonerInfoRepository summonerInfoRepository;
        private readonly RankedInfoRepository rankedInfoRepository;
        private readonly RiotApiRepository riotApiRepository;

        public SummonerInfoService(
            SummonerInfoRepository summonerInfoRepository,
            RankedInfoRepository rankedInfoRepository,
            RiotApiRepository riotApiRepository
        ) {
            this.summonerInfoRepository = summonerInfoRepository;
            this.riotApiRepository = riotApiRepository;
            this.rankedInfoRepository = rankedInfoRepository;
        }

        public async Task<SummonerInfoModel> GetSummonerInfoModel(string summonerName, Region region) {
            // First, query the information from the database
            var summonerInfoModel = await summonerInfoRepository.GetSummonerByUsernameAndRegion(summonerName, region);

            // If it is not null, return
            if (summonerInfoModel is not null) {
                return summonerInfoModel;
            }

            // Otherwise perform api call
            summonerInfoModel = await riotApiRepository.GetSummonerInfo(summonerName, region);
            if (summonerInfoModel is null) { // return null if it is null
                return null;
            }

            // if its not null also call api for ranked stats
            var rankedInfoModel =
                await riotApiRepository.GetRankedInfoModel(summonerInfoModel.EncryptedSummonerId, region);

            summonerInfoModel.RankedInfo = await rankedInfoRepository.Add(rankedInfoModel);
            summonerInfoModel = await summonerInfoRepository.Add(summonerInfoModel);
            return summonerInfoModel;
        }
    }
}