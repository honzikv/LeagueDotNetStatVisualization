using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services.Summoner {
    /// <summary>
    /// Tato sluzba poskytuje informace o specifickem hraci (summoner) bud zavolani Riot Api nebo ziskanim dat z db
    /// </summary>
    public class SummonerService {
        private readonly SummonerRepository summonerRepository;
        private readonly RiotApiRepository riotApiRepository;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;
        private readonly QueueInfoRepository queueInfoRepository;
        private readonly ApplicationUserRepository applicationUserRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<SummonerService> logger;

        public SummonerService(
            SummonerRepository summonerRepository,
            RiotApiRepository riotApiRepository,
            RiotApiUpdateConfig riotApiUpdateConfig,
            QueueInfoRepository queueInfoRepository,
            ApplicationUserRepository applicationUserRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<SummonerService> logger
        ) {
            this.summonerRepository = summonerRepository;
            this.riotApiRepository = riotApiRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.queueInfoRepository = queueInfoRepository;
            this.applicationUserRepository = applicationUserRepository;
            this.userManager = userManager;
            this.logger = logger;
        }

        public readonly Dictionary<string, string> QueryableServers = ServerConstants.QueryableServers;

        /// <summary>
        /// Ziska summoner info nebo vyhodi ActionNotSuccessfulException coz znamena ze uzivatel neexistuje
        /// </summary>
        /// <param name="summonerName">Uzivatelske jmeno</param>
        /// <param name="region">Server, pro ktery hledame uzivatele</param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<SummonerModel> GetSummoner(string summonerName, Region region) {
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
        /// Aktualizuje summoner data z Riot api
        /// </summary>
        /// <param name="summonerId">Id v databazi</param>
        /// <param name="writeUpdateTime">Zda-li se do summonera ma zapsat casove razitko pro posledni  update</param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<SummonerModel> UpdateSummoner(int summonerId, bool writeUpdateTime = false) {
            // Tracked entita z db, ktera se bude updatovat
            var dbSummoner = await summonerRepository.Get(summonerId);
            if (dbSummoner == null) {
                throw new RedirectToHomePageException("Error, user does not exist");
            }

            // Vypocet, zda-li lze profil aktualizovat
            var diff = DateTime.Now - dbSummoner.LastUpdate;

            // Pokud ne, vyhodime exception
            if (diff.TotalSeconds < riotApiUpdateConfig.MinUpdateTimeSpan.TotalSeconds) {
                throw new ActionNotSuccessfulException($"Error, user was updated {diff.Seconds} seconds ago," +
                                                       "it can be updated again " +
                                                       $"in {(riotApiUpdateConfig.MinUpdateTimeSpan - diff).Seconds} " +
                                                       "seconds");
            }
            
            // Ziskani aktualizovaneho profilu z Riot api
            var updatedSummoner =
                await riotApiRepository.GetSummonerInfoFromEncryptedSummonerId(dbSummoner.EncryptedSummonerId, Region.Get(dbSummoner.Region));
            
            // Ziskani queue info pro uzivatele
            var updatedQueueInfo =
                await riotApiRepository.GetQueueInfoList(dbSummoner.EncryptedSummonerId,
                    Region.Get(dbSummoner.Region));

            dbSummoner.UpdateFromApi(updatedSummoner);
            foreach (var queueInfo in dbSummoner.QueueInfoList) {
                await queueInfoRepository.Delete(queueInfo.Id);
            }

            foreach (var queueInfo in updatedQueueInfo) {
                queueInfo.Summoner = dbSummoner;
            }

            dbSummoner.QueueInfoList = updatedQueueInfo;

            if (writeUpdateTime) {
                dbSummoner.LastUpdate = DateTime.Now;
            }
            
            return await summonerRepository.Update(dbSummoner);
        }

        public async Task<bool> IsSummonerTaken(SummonerModel summoner) 
            => await applicationUserRepository.IsSummonerTaken(summoner);

        public async Task<OperationResult<string>> LinkSummonerToApplicationUser(ApplicationUser user, string summonerName, string server) {
            if (!QueryableServers.ContainsKey(server.ToLower())) {
                return new()  {
                    Error = true,
                    Message = "Error, server does not exist."
                };
            }

            var summoner = await GetSummoner(summonerName, Region.Get(server.ToUpper()));

            if (user.Summoner is not null && summoner.EncryptedAccountId == user.Summoner.EncryptedAccountId) {
                return new () {
                    Error = false,
                    Message = "You have already linked this summoner."
                };
            }

            if (await IsSummonerTaken(summoner)) {
                return new() {
                    Error = true,
                    Message = "Error, this summoner is already taken."
                };
            }

            user.Summoner = summoner;
            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded) {
                return new () {
                    Error = true,
                    Message = "Error, data could not be updated. Please try again later."
                };
            }

            return new() {
                Error = false,
                Message = "Profile sucessfully updated."
            };
        }
        
    }
}