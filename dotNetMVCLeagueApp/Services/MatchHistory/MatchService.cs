using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services.MatchHistory {
    public class MatchService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchRepository matchRepository;
        private readonly MatchInfoSummonerInfoRepository matchSummonerRepository;
        private readonly SummonerRepository summonerRepository;

        private readonly ILogger<MatchService> logger;

        public MatchService(RiotApiRepository riotApiRepository,
            MatchRepository matchRepository,
            MatchInfoSummonerInfoRepository matchSummonerRepository,
            SummonerRepository summonerRepository,
            ILogger<MatchService> logger
        ) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchRepository = matchRepository;
            this.matchSummonerRepository = matchSummonerRepository;
            this.summonerRepository = summonerRepository;
        }

        private const int MaxGamesFromApiPerRequest = 10;

        private readonly Dictionary<string, int> queueNameToQueueId = ServerConstants.QueueIdToQueueNames;

        public List<MatchModel> GetMatchHistory(SummonerModel summoner,
            int pageSize, int pageNumber, string queueType) {
            logger.LogDebug($"Getting info for {summoner.Name} @ {summoner.Region}");
            return matchRepository.GetNMatchesByQueueTypeAndDateTimeDesc(summoner, queueType, pageSize,
                pageNumber * pageSize);
        }

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

        public async Task<(List<MatchModel>, bool)> GetFilteredMatchHistory(SummonerModel summoner, int pageSize,
            int pageNumber, string queueType, bool update) {
            var matchHistory = queueType == ServerConstants.AllGamesDbValue
                ? matchRepository.GetNMatchesByDateTimeDesc(summoner, pageSize, pageNumber * pageSize)
                : matchRepository.GetNMatchesByQueueTypeAndDateTimeDesc(summoner, queueType, pageSize,
                    pageNumber * pageSize);
            var updated = false;

            if (matchHistory.Count < MaxGamesFromApiPerRequest && update) {
                var lastGamePlaytime =
                    matchHistory.Count == 0
                        ? DateTime.Now
                        : matchHistory[~1].PlayTime; // zjistime datum posledni hry - od toho budeme hledat
                var updateCount = pageSize - matchHistory.Count <= MaxGamesFromApiPerRequest
                    ? pageSize - matchHistory.Count
                    : MaxGamesFromApiPerRequest;

                var newGames = await UpdateMatchHistory(summoner, updateCount, lastGamePlaytime, queueType);
                if (newGames.Count > 0) {
                    updated = true;
                }

                matchHistory.AddRange(newGames);
            }

            return (matchHistory, updated);
        }

        public async Task<List<MatchModel>> UpdateMatchHistory(SummonerModel summoner, int numberOfGames,
            DateTime lastGamePlaytime, string queueType) {
            // hry z Riot api
            var result = new List<MatchModel>();
            var gamesFromApi = await riotApiRepository.GetMatchHistoryFromDateTimeDesc(summoner,
                Region.Get(summoner.Region), numberOfGames,
                lastGamePlaytime,
                queueType: queueNameToQueueId.ContainsKey(queueType)
                    ? ServerConstants.GetQueueId(queueType)
                    : null);

            foreach (var match in gamesFromApi) {
                result.Add(await AddOrUpdateMatch(summoner, match));
            }

            return result;
        }

        public async Task<List<MatchModel>> UpdateMatchHistory(SummonerModel summoner, int numberOfGames) {
            var result = new List<MatchModel>();
            var apiGames = await riotApiRepository.GetMatchListFromApi(summoner.EncryptedAccountId,
                Region.Get(summoner.Region), numberOfGames);

            foreach (var game in apiGames) {
                result.Add(await AddOrUpdateMatch(summoner, game));
            }

            return result;
        }
    }
}