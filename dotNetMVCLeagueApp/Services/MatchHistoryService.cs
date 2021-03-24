﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }

        /// <summary>
        /// Get list of matches for specified summoner
        /// </summary>
        /// <param name="summoner"></param>
        /// <param name="numberOfGames"></param>
        /// <returns></returns>
        public List<MatchInfoModel> GetGameMatchList(SummonerInfoModel summoner, int numberOfGames) {
            logger.LogDebug($"Getting games for {summoner}");
            return matchInfoRepository.GetLastNMatches(summoner, numberOfGames).ToList();
        }
    }
}