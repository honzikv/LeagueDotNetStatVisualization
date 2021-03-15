using System;
using System.Collections.Generic;
using System.Text;
using dotNetMVCLeagueApp.Models;
using dotNetMVCLeagueApp.Models.Match;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Data {
    public class LeagueDbContext : IdentityDbContext {
        public LeagueDbContext(DbContextOptions<LeagueDbContext> options)
            : base(options) {
        }

        public DbSet<SummonerInfoModel> SummonerInfoModels;

        public DbSet<MatchInfoModel> MatchInfoModels;

        public DbSet<ChampionBanModel> ChampionBanModels;
    }
}