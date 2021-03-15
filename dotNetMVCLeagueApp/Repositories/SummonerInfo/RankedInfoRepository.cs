﻿using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Repositories {
    public class RankedInfoRepository : EfCoreRepository<RankedInfoModel, LeagueDbContext> {
        public RankedInfoRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }
    }
}