using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Data {
    public class LeagueDbContext : DbContext {
        public LeagueDbContext(DbContextOptions<LeagueDbContext> options)
            : base(options) { }

        // Summoner Page
        public DbSet<SummonerInfoModel> SummonerInfoModels { get; set; }

        public DbSet<QueueInfoModel> QueueInfoModels { get; set; }

        public DbSet<OverviewStatsModel> OverviewStatsModels;

        // Match
        public DbSet<MatchInfoModel> MatchInfoModels { get; set; }

        public DbSet<ChampionBanModel> ChampionBanModels { get; set; }

        public DbSet<PlayerInfoModel> PlayerInfoModels { get; set; }

        public DbSet<TeamStatsInfoModel> TeamStatsInfoModels { get; set; }
    }
}