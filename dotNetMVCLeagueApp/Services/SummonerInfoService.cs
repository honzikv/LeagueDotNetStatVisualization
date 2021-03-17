using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exception;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Repositories.SummonerInfo;
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

        public SummonerInfoService(
            SummonerInfoRepository summonerInfoRepository,
            RankedInfoRepository queueInfoRepository,
            RiotApiRepository riotApiRepository,
            RiotApiUpdateConfig riotApiUpdateConfig
        ) {
            this.summonerInfoRepository = summonerInfoRepository;
            this.riotApiRepository = riotApiRepository;
            this.queueInfoRepository = queueInfoRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
        }

        public async Task<SummonerInfoModel> GetSummonerInfo(string summonerName, Region region) {
            // First, query the information from the database
            var summonerInfo = await summonerInfoRepository.GetSummonerByUsernameAndRegion(summonerName, region);

            // If it is not null, return
            if (summonerInfo is not null) {
                return summonerInfo;
            }

            // Otherwise create a new info (if it exists in league api)
            summonerInfo = await riotApiRepository.GetSummonerInfo(summonerName, region);
            if (summonerInfo is null) { // return null if it does not exist
                return null;
            }

            // if its not null also call api for ranked stats
            var queueInfoModels =
                await riotApiRepository.GetRankedInfoList(summonerInfo.EncryptedSummonerId, region);

            summonerInfo.QueueInfo = await queueInfoRepository.AddAll(queueInfoModels);
            return await summonerInfoRepository.Add(summonerInfo); // Save summonerInfoModel and return it
        }

        private void UpdateQueueInfoList(List<QueueInfoModel> oldQueueInfo, List<QueueInfoModel> newQueueInfo) {
            foreach (var queueInfo in newQueueInfo) {
                var existingQueueInfo =
                    oldQueueInfo.Find(info => info.QueueType == queueInfo.QueueType);

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

            var updatedSummonerInfo =
                await riotApiRepository.GetSummonerInfo(oldSummonerInfo.Name, Region.Get(oldSummonerInfo.Region));

            var updatedQueueInfo =
                await riotApiRepository.GetRankedInfoList(oldSummonerInfo.Name, Region.Get(oldSummonerInfo.Region));

            // Set Ids for queue info and summoner model
            UpdateQueueInfoList(oldSummonerInfo.QueueInfo, updatedQueueInfo);
            updatedSummonerInfo.Id = oldSummonerInfo.Id;
            updatedSummonerInfo.QueueInfo =
                await queueInfoRepository.AddAll(updatedQueueInfo); // assign updated queue info

            return await summonerInfoRepository.Add(updatedSummonerInfo);
        }
    }
}