using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Data {
    /// <summary>
    /// Kontext pro pripojeni k databazi
    /// </summary>
    public class LeagueDbContext : DbContext {
        public LeagueDbContext(DbContextOptions<LeagueDbContext> options)
            : base(options) {
        }

        // Summoner Page
        public DbSet<SummonerInfoModel> SummonerInfoModels { get; set; }

        public DbSet<QueueInfoModel> QueueInfoModels { get; set; }

        public DbSet<OverviewStatsModel> OverviewStatsModels;

        // Match
        public DbSet<MatchInfoModel> MatchInfoModels { get; set; }

        public DbSet<ChampionBanModel> ChampionBanModels { get; set; }

        public DbSet<PlayerInfoModel> PlayerInfoModels { get; set; }

        public DbSet<TeamStatsInfoModel> TeamStatsInfoModels { get; set; }
        public DbSet<MatchInfoSummonerInfo> MatchInfoSummonerInfos { get; set; }

        // Implementace M : N pro summoner a match
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Override to implement M : N for MatchInfo and SummonerInfo model objects
            modelBuilder.Entity<MatchInfoSummonerInfo>()
                .HasKey(matchSummoner => new {
                    matchSummoner.MatchInfoModelId,
                    matchSummoner.SummonerInfoModelId
                });

            modelBuilder.Entity<MatchInfoSummonerInfo>()
                .HasOne(matchSummoner => matchSummoner.MatchInfo)
                .WithMany(matchInfo => matchInfo.SummonerInfoList)
                .HasForeignKey(matchInfo => matchInfo.MatchInfoModelId);
            
        }
    }
}