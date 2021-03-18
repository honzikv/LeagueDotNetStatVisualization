using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Repositories {
    /// <summary>
    /// This repository wraps functionality of RiotApi object for better abstraction
    /// </summary>
    public class RiotApiRepository {
        private readonly RiotApi riotApi;

        private readonly IMapper mapper;

        public RiotApiRepository(RiotApi riotApi, IMapper mapper) {
            this.riotApi = riotApi;
            this.mapper = mapper;
        }

        public async Task<SummonerInfoModel> GetSummonerInfo(string summonerName, Region region) {
            var summoner = await riotApi.SummonerV4.GetBySummonerNameAsync(region, summonerName);
            if (summoner is null) { // return null, this means that the user does not exist
                return null;
            }

            // Map to model object
            return new SummonerInfoModel {
                SummonerLevel = summoner.SummonerLevel,
                Name = summoner.Name,
                ProfileIconId = summoner.ProfileIconId,
                EncryptedSummonerId = summoner.Id,
                Region = region.Key,
                LastUpdate = DateTime.Now
            };
        }

        /// <summary>
        /// Get list of QueueInfoModels that contain information about flexq and soloq
        /// </summary>
        /// <param name="encryptedSummonerId">Encrypted summoner id</param>
        /// <param name="region">Region of the user</param>
        /// <returns>List of QueueInfoModels</returns>
        public async Task<List<QueueInfoModel>> GetRankedInfoList(string encryptedSummonerId, Region region) {
            var leagueEntries =
                await riotApi.LeagueV4.GetLeagueEntriesForSummonerAsync(region, encryptedSummonerId);

            // Return empty ranked info model since the use has not played any ranked
            if (leagueEntries is null) {
                return new();
            }

            // Iterate over all entries, map flex queue and solo queue to the rankedInfoModel object
            return leagueEntries.Where(leagueEntry => leagueEntry.QueueType == LeagueEntryConst.RankedFlex ||
                                                      leagueEntry.QueueType == LeagueEntryConst.RankedSolo)
                .Select(leagueEntry => mapper.Map<QueueInfoModel>(leagueEntry))
                .ToList();
        }
    }
}