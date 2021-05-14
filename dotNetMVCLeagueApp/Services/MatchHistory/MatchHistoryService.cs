using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services.MatchHistory {
    /// <summary>
    /// Sluzba pro ziskani informaci o hrach
    /// </summary>
    public class MatchHistoryService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchRepository matchRepository;
        private readonly MatchInfoSummonerInfoRepository matchSummonerRepository;
        private readonly SummonerRepository summonerRepository;

        private readonly ILogger<MatchHistoryService> logger;

        public MatchHistoryService(
            RiotApiRepository riotApiRepository,
            MatchRepository matchRepository,
            MatchInfoSummonerInfoRepository matchSummonerRepository,
            SummonerRepository summonerRepository,
            ILogger<MatchHistoryService> logger
        ) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchRepository = matchRepository;
            this.matchSummonerRepository = matchSummonerRepository;
            this.summonerRepository = summonerRepository;
        }
        

        /// <summary>
        /// Ziska seznam zapasu pro daneho uzivatele
        /// </summary>
        /// <param name="summoner">summoner, pro ktereho match list hledame</param>
        /// <param name="numberOfGames">pocet her</param>
        /// <returns></returns>
        public List<MatchModel> GetMatchlist(SummonerModel summoner, int numberOfGames) {
            logger.LogDebug($"Getting games for {summoner}");

            // Pokud byl v DateTime.MinValue tak to indikuje, ze je summoner prave ziskany z api,
            // takze muzeme zapasy aktualizovat, jinak pouze prineseme posledni z db
            return summoner.LastUpdate == DateTime.MinValue
                ? UpdateGameMatchListAsync(summoner, numberOfGames)
                : GetNMatches(summoner, numberOfGames);
        }

        public List<MatchModel> GetMatchlist(SummonerModel summoner, int numberOfGames, string queueType) {
            return GetNMatches(summoner, numberOfGames, queueType);
        }

        private List<MatchModel> GetNMatches(SummonerModel summoner, int numberOfGames)
            => matchRepository.GetNMatchesByDateTimeDesc(summoner, numberOfGames);

        private List<MatchModel> GetNMatches(SummonerModel summoner, int numberOfGames, string queueType)
            => matchRepository.GetNMatchesByQueueTypeAndDateTimeDesc(summoner, queueType, numberOfGames);

        /// <summary>
        /// Pridani nebo update MatchInfo
        /// </summary>
        /// <param name="summoner">Summoner pro ktereho provadime update</param>
        /// <param name="apiMatch">Match info z api</param>
        /// <returns></returns>
        private async Task<MatchModel> AddOrUpdateMatch(SummonerModel summoner,
            MatchModel apiMatch) {
            // Zkusime pridat match info do db pokud jeste nebylo pridano a ziskame vysledek
            var match = await matchRepository.Get(apiMatch.Id);

            if (match is null) {
                var matchTimelineModel =
                    await riotApiRepository.GetMatchTimelineFromApi(apiMatch.Id, Region.Get(summoner.Region));
                apiMatch.MatchTimelineModel = matchTimelineModel;
                match = await matchRepository.Add(apiMatch);
            }

            logger.LogDebug($"Summoner: {summoner}, matchInfo: {match.Id}");

            // Pridame link pokud neexistuje
            if (!await matchSummonerRepository.AnyJoinBetweenMatchSummoner(match.Id, summoner.Id)) {
                await matchSummonerRepository.Add(new MatchToSummonerModel {
                    MatchInfoModelId = match.Id,
                    SummonerInfoModelId = summoner.Id,
                    Match = match,
                    Summoner = summoner
                });
            }

            return await matchRepository.Get(apiMatch.Id);
        }

        /// <summary>
        /// Synchronizace funkce UpdateGameMatchList pro controller
        /// </summary>
        /// <param name="summoner"></param>
        /// <param name="numberOfGames"></param>
        /// <returns></returns>
        public List<MatchModel> UpdateGameMatchListAsync(SummonerModel summoner, int numberOfGames)
            => UpdateMatchList(summoner, numberOfGames).GetAwaiter().GetResult();

        private async Task<List<MatchModel>> UpdateMatchList(SummonerModel summoner, int numberOfGames) {
            var matchList = await riotApiRepository.GetMatchListFromApi(summoner.EncryptedAccountId,
                Region.Get(summoner.Region), numberOfGames);

            var result = new List<MatchModel>();
            foreach (var match in matchList) {
                result.Add(await AddOrUpdateMatch(summoner, match));
            }

            summoner.LastUpdate = DateTime.Now;
            await summonerRepository.Update(summoner);

            return result;
        }

        private async Task<List<MatchModel>> GetMatchlist(SummonerModel summoner, int numberOfGames, int skip) {
            var matchList = matchRepository.GetNMatchesByDateTimeDesc(summoner, numberOfGames, skip);
            
            if (matchList.Count == numberOfGames) {
                return matchList;
            }

            var oldestDateMatch = matchList[~1]; // nejstarsi datum je posledni, to pouzijeme k ziskani
            // her z api, protoze muze byt pouzito jako parametr

            var count = numberOfGames - matchList.Count;

            var matchesFromApi = await riotApiRepository.GetMatchListFromApiByDateTimeDescending(
                summoner.EncryptedAccountId, Region.Get(summoner.Region), numberOfGames, oldestDateMatch.PlayTime);
            
            // Zde "aktualizaci" nepocitame
            var result = new List<MatchModel>();
            foreach (var match in matchList) {
                result.Add(await AddOrUpdateMatch(summoner, match));
            }

            return result;
        }
        
    }
}