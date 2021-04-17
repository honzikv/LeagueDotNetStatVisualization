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
    /// Tato sluzba poskytuje informace o specifickem hraci (summoner) bud zavolani Riot Api nebo ziskanim dat z db
    /// </summary>
    public class SummonerInfoService {
        private readonly SummonerInfoEntityRepository summonerInfoEntityRepository;
        private readonly RiotApiRepository riotApiRepository;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;
        private readonly QueueInfoRepository queueInfoRepository;

        private readonly ILogger<SummonerInfoService> logger;

        public SummonerInfoService(
            SummonerInfoEntityRepository summonerInfoEntityRepository,
            RiotApiRepository riotApiRepository,
            RiotApiUpdateConfig riotApiUpdateConfig,
            QueueInfoRepository queueInfoRepository,
            ILogger<SummonerInfoService> logger
        ) {
            this.summonerInfoEntityRepository = summonerInfoEntityRepository;
            this.riotApiRepository = riotApiRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.queueInfoRepository = queueInfoRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Ziska summoner info nebo vyhodi ActionNotSuccessfulException coz znamena ze uzivatel neexistuje
        /// </summary>
        /// <param name="summonerName">Uzivatelske jmeno</param>
        /// <param name="region">Server, pro ktery hledame uzivatele</param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<SummonerInfoModel> GetSummonerInfo(string summonerName, Region region) {
            logger.LogDebug($"Fetching summoner info for {region.Key} {summonerName}");
            // Nejprve provedeme query do db zda-li jsme summonera uz nekdy predtim nenacitali
            var summonerInfo = await summonerInfoEntityRepository.GetSummonerByUsernameAndRegion(summonerName, region);

            // Pokud neni summoner info null vratime
            if (summonerInfo is not null) {
                return summonerInfo;
            }

            logger.LogDebug(
                $"Summoner info was null, trying to fetch from API summonerName={summonerName}, region={region.Key}");
            // Jinak ziskame z API
            summonerInfo = await riotApiRepository.GetSummonerInfo(summonerName, region);
            if (summonerInfo is null) { // Vyhodime exception pokud summoner neexistuje
                throw new ActionNotSuccessfulException($"User {summonerName} on server {region.Key} does not exist!");
            }

            logger.LogDebug($"Fetched: {summonerInfo}");
            // Pokud summoner neni null tak zavolame api pro ranked statistiky 
            summonerInfo.QueueInfo =
                await riotApiRepository.GetQueueInfoList(summonerInfo.EncryptedSummonerId, region);
            return await summonerInfoEntityRepository.Add(summonerInfo); // ulozime do db a vratime
        }

        /// <summary>
        /// Aktualizuje queue info seznam spolu s predchozimi id (pokud existuji)
        /// </summary>
        /// <param name="oldQueueInfo">Stary seznam</param>
        /// <param name="newQueueInfo">Novy seznam</param>
        private void UpdateQueueInfoList(ICollection<QueueInfoModel> oldQueueInfo,
            ICollection<QueueInfoModel> newQueueInfo) {
            foreach (var queueInfo in newQueueInfo) {
                
                var existingQueueInfo =
                    oldQueueInfo.FirstOrDefault(info => info.QueueType == queueInfo.QueueType);

                // K aktualizovane hodnote priradime id pokud neni null
                if (existingQueueInfo is not null) {
                    queueInfo.Id = existingQueueInfo.Id;
                }
            }
        }

        /// <summary>
        /// Aktualizuje summoner info z Riot api
        /// </summary>
        /// <param name="summonerId"></param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<SummonerInfoModel> UpdateSummonerInfo(int summonerId) {
            // Tracked entita z db, ktera se bude updatovat
            var dbSummonerInfo = await summonerInfoEntityRepository.Get(summonerId);
            if (dbSummonerInfo == null) {
                throw new ActionNotSuccessfulException("Error, user does not exist");
            }

            // Vypocet, zda-li lze profil aktualizovat
            var diff = DateTime.Now - dbSummonerInfo.LastUpdate;

            // Pokud ne, vyhodime exception
            if (diff < riotApiUpdateConfig.MinUpdateTimeSpan) {
                throw new ActionNotSuccessfulException($"Error, user was updated {diff.Seconds} seconds ago," +
                                                       "it can be updated again " +
                                                       $"in {(riotApiUpdateConfig.MinUpdateTimeSpan - diff).Seconds} " +
                                                       "seconds");
            }
            
            logger.LogDebug("Trying to get updated summoner info");

            // Ziskani aktualizovaneho profilu z Riot api
            var updatedSummonerInfo =
                await riotApiRepository.GetSummonerInfo(dbSummonerInfo.Name, Region.Get(dbSummonerInfo.Region));

            logger.LogDebug("Updated summoner info from api, getting queue info");

            // Ziskani queue info pro uzivatele
            var updatedQueueInfo =
                await riotApiRepository.GetQueueInfoList(dbSummonerInfo.EncryptedSummonerId,
                    Region.Get(dbSummonerInfo.Region));
            
            logger.LogDebug("Updated queue info from api, updating in db");
            
            dbSummonerInfo.UpdateFromApi(updatedSummonerInfo);
            foreach (var queueInfo in dbSummonerInfo.QueueInfo) {
                await queueInfoRepository.Delete(queueInfo.Id);
            }

            foreach (var queueInfo in updatedQueueInfo) {
                queueInfo.SummonerInfo = dbSummonerInfo;
            }

            dbSummonerInfo.QueueInfo = updatedQueueInfo;
            return await summonerInfoEntityRepository.Update(dbSummonerInfo);
        }
    }
}