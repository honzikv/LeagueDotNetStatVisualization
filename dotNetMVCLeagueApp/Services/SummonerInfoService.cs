using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// This service provides information about specific summoner either by calling a RiotApi or getting data from
    /// database
    /// </summary>
    public class SummonerInfoService {
        private readonly SummonerInfoRepository summonerInfoRepository;
        private readonly RiotApiRepository riotApiRepository;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;

        private readonly ILogger<SummonerInfoService> logger;

        public SummonerInfoService(
            SummonerInfoRepository summonerInfoRepository,
            RiotApiRepository riotApiRepository,
            RiotApiUpdateConfig riotApiUpdateConfig,
            ILogger<SummonerInfoService> logger
        ) {
            this.summonerInfoRepository = summonerInfoRepository;
            this.riotApiRepository = riotApiRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.logger = logger;
        }

        /// <summary>
        /// Obtains summoner info or throws an ActionNotSuccessfulException which means that the summoner does not exist
        /// </summary>
        /// <param name="summonerName"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<SummonerInfoModel> GetSummonerInfo(string summonerName, Region region) {
            logger.LogDebug($"Fetching summoner info for {region.Key} {summonerName}");
            // First, query the information from the database
            var summonerInfo = await summonerInfoRepository.GetSummonerByUsernameAndRegion(summonerName, region);

            // If it is not null, return
            if (summonerInfo is not null) {
                return summonerInfo;
            }

            logger.LogDebug(
                $"Summoner info was null, trying to fetch from API summonerName={summonerName}, region={region.Key}");
            // Otherwise create a new info (if it exists in league api)
            summonerInfo = await riotApiRepository.GetSummonerInfo(summonerName, region);
            if (summonerInfo is null) { // throw exception if it does not exist
                throw new ActionNotSuccessfulException($"User {summonerName} on server {region.Key} does not exist!");
            }

            logger.LogDebug($"Fetched: {summonerInfo}");
            // if its not null also call api for ranked stats
            summonerInfo.QueueInfo =
                await riotApiRepository.GetQueueInfoList(summonerInfo.EncryptedSummonerId, region);
            return await summonerInfoRepository.Add(summonerInfo); // Save summonerInfoModel and return it
        }

        /// <summary>
        /// Updates queue info list with previous ids if they exist
        /// </summary>
        /// <param name="oldQueueInfo">List with old values</param>
        /// <param name="newQueueInfo">List with new values</param>
        private void UpdateQueueInfoList(ICollection<QueueInfoModel> oldQueueInfo,
            ICollection<QueueInfoModel> newQueueInfo) {
            foreach (var queueInfo in newQueueInfo) {
                var existingQueueInfo =
                    oldQueueInfo.FirstOrDefault(info => info.QueueType == queueInfo.QueueType);

                // Assign Id if it exists
                if (existingQueueInfo is not null) {
                    queueInfo.Id = existingQueueInfo.Id;
                }
            }
        }

        public async Task<SummonerInfoModel> UpdateSummonerInfo(int summonerId) {
            var oldSummonerInfo = await summonerInfoRepository.Get(summonerId);
            if (oldSummonerInfo == null) {
                throw new ActionNotSuccessfulException("Error, user does not exist");
            }

            // Calculate diff to determine if the profile can be updated
            var diff = DateTime.Now - oldSummonerInfo.LastUpdate;

            // If not, throw an exception with corresponding message
            if (diff < riotApiUpdateConfig.MinUpdateTimeSpan) {
                throw new ActionNotSuccessfulException($"Error, user was updated {diff.Seconds} seconds ago," +
                                                       "it can be updated again " +
                                                       $"in {(riotApiUpdateConfig.MinUpdateTimeSpan - diff).Seconds} " +
                                                       "seconds");
            }

            // Get updated summoner info from Riot api
            var updatedSummonerInfo =
                await riotApiRepository.GetSummonerInfo(oldSummonerInfo.Name, Region.Get(oldSummonerInfo.Region));

            // Get updated queue info from Riot api
            var updatedQueueInfo =
                await riotApiRepository.GetQueueInfoList(oldSummonerInfo.EncryptedSummonerId,
                    Region.Get(oldSummonerInfo.Region));

            // Set Ids for queue info and summoner model
            UpdateQueueInfoList(oldSummonerInfo.QueueInfo, updatedQueueInfo);
            updatedSummonerInfo.Id = oldSummonerInfo.Id;
            updatedSummonerInfo.QueueInfo = updatedQueueInfo; // assign updated queue info

            return await summonerInfoRepository.Add(updatedSummonerInfo);
        }
    }
}