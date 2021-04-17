﻿using System.Collections.Generic;
using System.Linq;
using LeagueStatAppReact.Data;
using LeagueStatAppReact.Data.Models.Match;
using LeagueStatAppReact.Data.Models.SummonerPage;

namespace LeagueStatAppReact.Repositories {
    public class MatchInfoRepository : EfCoreRepository<MatchInfoModel, LeagueDbContext> {
        public MatchInfoRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        public IEnumerable<MatchInfoModel> GetLastNMatches(SummonerInfoModel summoner, int n, int start = 0) =>
            LeagueDbContext.MatchInfoModels.Where(matchInfo => matchInfo.SummonerInfoModel.Id == summoner.Id)
                .OrderByDescending(matchInfo => matchInfo.PlayTime) // Sort by date time so the newest are first
                .Skip(start) // Offset start if present
                .Take(n); // Take maximum of N elements
    }
}