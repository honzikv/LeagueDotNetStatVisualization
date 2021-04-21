﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    public class MatchInfoEntityRepository : EfCoreEntityRepository<MatchInfoModel, LeagueDbContext> {
        public MatchInfoEntityRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        /// <summary>
        /// Ziska poslednich N zapasu podle nejnovejsiho datumu pro daneho summonera
        /// </summary>
        /// <param name="summoner">Summoner, ktery se v zapasech vyskytoval</param>
        /// <param name="n">Pocet zapasu</param>
        /// <param name="start">Zacatek</param>
        /// <returns>Seznam s N nebo mene entitami</returns>
        public IEnumerable<MatchInfoModel> GetNLastMatches(SummonerInfoModel summoner, int n, int start = 0) =>
            LeagueDbContext.MatchInfoSummonerInfos
                .Where(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id)
                .OrderByDescending(match => match.MatchInfo.PlayTime)
                .Skip(start)
                .Take(n) // Take maximum of N elements
                .ToList()
                .ConvertAll(matchSummoner => matchSummoner.MatchInfo);

    }
}