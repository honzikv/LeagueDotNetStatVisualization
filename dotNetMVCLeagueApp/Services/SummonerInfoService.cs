﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exception;
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
        private readonly RankedInfoRepository queueInfoRepository;
        private readonly RiotApiRepository riotApiRepository;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;

        private readonly ILogger<SummonerInfoService> logger;

        public SummonerInfoService(
            SummonerInfoRepository summonerInfoRepository,
            RankedInfoRepository queueInfoRepository,
            RiotApiRepository riotApiRepository,
            RiotApiUpdateConfig riotApiUpdateConfig,
            ILogger<SummonerInfoService> logger
        ) {
            this.summonerInfoRepository = summonerInfoRepository;
            this.riotApiRepository = riotApiRepository;
            this.queueInfoRepository = queueInfoRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.logger = logger;
        }

        public async Task<SummonerInfoModel> GetSummonerInfo(string summonerName, Region region) {
            logger.LogInformation($"Fetching summoner info for {region.Key} {summonerName}");
            // First, query the information from the database
            var summonerInfo = await summonerInfoRepository.GetSummonerByUsernameAndRegion(summonerName, region);

            // If it is not null, return
            if (summonerInfo is not null) {
                return summonerInfo;
            }

            // Otherwise create a new info (if it exists in league api)
            summonerInfo = await riotApiRepository.GetSummonerInfo(summonerName, region);
            if (summonerInfo is null) { // throw exception if it does not exist
                throw new ActionNotSuccessfulException($"User {summonerName} on server {region.Key} does not exist!");
            }

            // if its not null also call api for ranked stats
            summonerInfo.QueueInfo =
                await riotApiRepository.GetRankedInfoList(summonerInfo.EncryptedSummonerId, region);
            return await summonerInfoRepository.Add(summonerInfo); // Save summonerInfoModel and return it
        }

        private void UpdateQueueInfoList(ICollection<QueueInfoModel> oldQueueInfo, ICollection<QueueInfoModel> newQueueInfo) {
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
                                                       $"it can be updated again " +
                                                       $"in {(riotApiUpdateConfig.MinUpdateTimeSpan - diff).Seconds}");
            }

            // Get updated summoner info from Riot api
            var updatedSummonerInfo =
                await riotApiRepository.GetSummonerInfo(oldSummonerInfo.Name, Region.Get(oldSummonerInfo.Region));

            // Get updated queue info from Riot api
            var updatedQueueInfo =
                await riotApiRepository.GetRankedInfoList(oldSummonerInfo.EncryptedSummonerId,
                    Region.Get(oldSummonerInfo.Region));

            // Set Ids for queue info and summoner model
            UpdateQueueInfoList(oldSummonerInfo.QueueInfo, updatedQueueInfo);
            updatedSummonerInfo.Id = oldSummonerInfo.Id;
            updatedSummonerInfo.QueueInfo =
                await queueInfoRepository.AddAll(updatedQueueInfo); // assign updated queue info

            return await summonerInfoRepository.Add(updatedSummonerInfo);
        }
    }
}