﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services.Summoner {
    /// <summary>
    /// Tato sluzba poskytuje informace o specifickem hraci (summoner) bud zavolani Riot Api nebo ziskanim dat z db
    /// </summary>
    public class SummonerInfoService {
        private readonly SummonerRepository summonerRepository;
        private readonly RiotApiRepository riotApiRepository;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;
        private readonly QueueInfoRepository queueInfoRepository;
        private readonly ApplicationUserRepository applicationUserRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<SummonerInfoService> logger;

        public SummonerInfoService(
            SummonerRepository summonerRepository,
            RiotApiRepository riotApiRepository,
            RiotApiUpdateConfig riotApiUpdateConfig,
            QueueInfoRepository queueInfoRepository,
            ApplicationUserRepository applicationUserRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<SummonerInfoService> logger
        ) {
            this.summonerRepository = summonerRepository;
            this.riotApiRepository = riotApiRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.queueInfoRepository = queueInfoRepository;
            this.applicationUserRepository = applicationUserRepository;
            this.logger = logger;
        }

        public Dictionary<string, string> GetQueryableServers => GameConstants.QueryableServers;

        /// <summary>
        /// Synchronizovana verze metody pro controller aby nemusel volat GetAwaiter a GetResult
        /// </summary>
        /// <param name="summonerName">Uzivatelske jmeno</param>
        /// <param name="region">Server, pro ktery hledame uzivatele</param>
        /// <returns></returns>
        public SummonerModel GetSummonerInfoAsync(string summonerName, Region region) =>
            GetSummonerInfo(summonerName, region).GetAwaiter().GetResult();

        /// <summary>
        /// Ziska summoner info nebo vyhodi ActionNotSuccessfulException coz znamena ze uzivatel neexistuje
        /// </summary>
        /// <param name="summonerName">Uzivatelske jmeno</param>
        /// <param name="region">Server, pro ktery hledame uzivatele</param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        private async Task<SummonerModel> GetSummonerInfo(string summonerName, Region region) {
            logger.LogDebug($"Fetching summoner info for {region.Key} {summonerName}");
            // Nejprve provedeme query do db zda-li jsme summonera uz nekdy predtim nenacitali
            var summonerInfo = await summonerRepository.GetSummonerByUsernameAndRegion(summonerName, region);

            // Pokud neni summoner info null vratime
            if (summonerInfo is not null) {
                return summonerInfo;
            }

            logger.LogDebug(
                $"Summoner info was null, trying to fetch from API summonerName={summonerName}, region={region.Key}");
            // Jinak ziskame z API
            summonerInfo = await riotApiRepository.GetSummonerInfo(summonerName, region);
            if (summonerInfo is null) { // Vyhodime exception pokud summoner neexistuje
                throw new RedirectToHomePageException($"User {summonerName} on server {region.Key} does not exist!");
            }

            logger.LogDebug($"Fetched: {summonerInfo}");
            // Pokud summoner neni null tak zavolame api pro ranked statistiky 
            summonerInfo.QueueInfoList =
                await riotApiRepository.GetQueueInfoList(summonerInfo.EncryptedSummonerId, region);

            return await summonerRepository.Add(summonerInfo); // ulozime do db a vratime
        }

        /// <summary>
        /// Synchronizace pro controller, aby nemusel volat GetAwaiter a GetResult
        /// </summary>
        /// <param name="summonerId"></param>
        /// <returns></returns>
        public SummonerModel UpdateSummonerInfoAsync(int summonerId)
            => UpdateSummoner(summonerId).GetAwaiter().GetResult();
        

        /// <summary>
        /// Aktualizuje summoner data z Riot api
        /// </summary>
        /// <param name="summonerId">Id v databazi</param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        private async Task<SummonerModel> UpdateSummoner(int summonerId) {
            // Tracked entita z db, ktera se bude updatovat
            var dbSummonerInfo = await summonerRepository.Get(summonerId);
            if (dbSummonerInfo == null) {
                throw new RedirectToHomePageException("Error, user does not exist");
            }

            // Vypocet, zda-li lze profil aktualizovat
            var diff = DateTime.Now - dbSummonerInfo.LastUpdate;

            // Pokud ne, vyhodime exception
            if (diff.TotalSeconds < riotApiUpdateConfig.MinUpdateTimeSpan.TotalSeconds) {
                throw new ActionNotSuccessfulException($"Error, user was updated {diff.Seconds} seconds ago," +
                                                       "it can be updated again " +
                                                       $"in {(riotApiUpdateConfig.MinUpdateTimeSpan - diff).Seconds} " +
                                                       "seconds");
            }


            // Ziskani aktualizovaneho profilu z Riot api
            var updatedSummonerInfo =
                await riotApiRepository.GetSummonerInfoFromEncryptedSummonerId(dbSummonerInfo.EncryptedSummonerId, Region.Get(dbSummonerInfo.Region));
            
            // Ziskani queue info pro uzivatele
            var updatedQueueInfo =
                await riotApiRepository.GetQueueInfoList(dbSummonerInfo.EncryptedSummonerId,
                    Region.Get(dbSummonerInfo.Region));

            dbSummonerInfo.UpdateFromApi(updatedSummonerInfo);
            foreach (var queueInfo in dbSummonerInfo.QueueInfoList) {
                await queueInfoRepository.Delete(queueInfo.Id);
            }

            foreach (var queueInfo in updatedQueueInfo) {
                queueInfo.Summoner = dbSummonerInfo;
            }

            dbSummonerInfo.QueueInfoList = updatedQueueInfo;
            return await summonerRepository.Update(dbSummonerInfo);
        }

        public async Task<bool> IsSummonerTaken(SummonerModel summoner) 
            => await applicationUserRepository.IsSummonerTaken(summoner);

        public bool IsSummonerTakenAsync(SummonerModel summoner) => IsSummonerTaken(summoner).GetAwaiter().GetResult();
    }
}