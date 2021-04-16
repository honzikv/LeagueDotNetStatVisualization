using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services {
    public class MatchHistoryService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchInfoRepository matchInfoRepository;

        private readonly ILogger<MatchHistoryService> logger;

        public MatchHistoryService(RiotApiRepository riotApiRepository, MatchInfoRepository matchInfoRepository,
            ILogger<MatchHistoryService> logger) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchInfoRepository = matchInfoRepository;
        }

        /// <summary>
        /// Get list of matches for specified summoner
        /// </summary>
        /// <param name="summoner"></param>
        /// <param name="numberOfGames"></param>
        /// <returns></returns>
        public List<MatchInfoModel> GetGameMatchList(SummonerInfoModel summoner, int numberOfGames) {
            logger.LogDebug($"Getting games for {summoner}");
            var games = matchInfoRepository.GetNLastMatches(summoner, numberOfGames).ToList();
            return games; // return list
        }

        private async Task<MatchInfoModel> AddOrUpdateMatchInfo(SummonerInfoModel summonerInfo,
            MatchInfoModel apiMatchInfo) {
            // Try get match info from database, if it has not been added yet add it and get the result
            var matchInfo = await matchInfoRepository.Get(apiMatchInfo.Id) ??
                            await matchInfoRepository.Add(apiMatchInfo);

            // Add link if it does not exist
            var matchSummoner = await matchInfoRepository.FindMatchInfoSummonerInfo(matchInfo.Id, summonerInfo.Id);
            if (matchSummoner is null) {
                await matchInfoRepository.LinkMatchInfoToSummonerInfo(new MatchInfoSummonerInfo {
                    MatchInfoModelId = matchInfo.Id,
                    SummonerInfoModelId = summonerInfo.Id,
                    MatchInfo = matchInfo,
                    SummonerInfo = summonerInfo
                });
            }

            return await matchInfoRepository.Get(apiMatchInfo.Id);
        }

        /// <summary>
        /// Get match list from Riot api and save it to the database
        /// </summary>
        /// <param name="summoner">Summoner for which the matches are queried</param>
        /// <param name="numberOfGames">Number of games to load (from newest)</param>
        /// <returns></returns>
        public async Task<List<MatchInfoModel>> UpdateGameMatchList(SummonerInfoModel summoner, int numberOfGames) {
            // Await from api
            var games = await riotApiRepository.GetMatchListFromApi(summoner.EncryptedAccountId,
                Region.Get(summoner.Region),
                numberOfGames);

            // Some records of the games might already exist in the database and therefore we just need to add  the summoner
            // to the specific game (games do not update so only adding the summoner is necessary)
            var tasks = new List<Task<MatchInfoModel>>(games.Count);
            foreach (var game in games) {
                tasks.Add(AddOrUpdateMatchInfo(summoner, game)); // for each game either add it to the database or
                // update the join table MatchInfoSummonerInfo
            }

            var result = await Task.WhenAll(tasks); // wait until all tasks are done
            return result.ToList();
        }
    }
}