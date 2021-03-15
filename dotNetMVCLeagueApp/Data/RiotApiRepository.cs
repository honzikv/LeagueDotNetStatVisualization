using System;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Models;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Data {
    /// <summary>
    /// This repository wraps functionality of RiotApi object for better abstraction
    /// </summary>
    public class RiotApiRepository {
        private readonly RiotApi riotApi;

        public RiotApiRepository(RiotApi riotApi) {
            this.riotApi = riotApi;
        }

        public async Task<SummonerInfoModel> GetSummonerInfo(string summonerName, Region region) {
            var summoner = await riotApi.SummonerV4.GetBySummonerNameAsync(region, summonerName);
            if (summoner is null) {
                return null;
            }

            // Map to model object
            return new SummonerInfoModel {
                Level = summoner.SummonerLevel,
                Name = summoner.Puuid,
                ProfileIconId = summoner.ProfileIconId,
                EncryptedSummonerId = summoner.Id,
                Region = region.Key,
                LastUpdate = DateTime.Now
            };
        }

        public async Task<RankedInfoModel> GetRankedInfoModel(string encryptedSummonerId, Region region) {
            var rankedInfoModel = new RankedInfoModel();
            var leagueEntries =
                (await riotApi.LeagueV4.GetLeagueEntriesForSummonerAsync(region, encryptedSummonerId);

            if (leagueEntries is null) {
                return rankedInfoModel;
            }

            
            foreach (var leagueEntry in leagueEntries) {
                
            }
        }
    }
}