using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Data {
    /// <summary>
    /// Kontext pro pripojeni k databazi
    /// </summary>
    public class LeagueDbContext : DbContext {
        public LeagueDbContext(DbContextOptions<LeagueDbContext> options)
            : base(options) { }

        /// <summary>
        /// Vsechny tabulky 
        /// </summary>
        public DbSet<SummonerInfoModel> SummonerInfoModels { get; set; }

        public DbSet<QueueInfoModel> QueueInfoModels { get; set; }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<ProfileCardModel> ProfileCardModels { get; set; }

        public DbSet<MatchInfoModel> MatchInfoModels { get; set; }

        public DbSet<PlayerInfoModel> PlayerInfoModels { get; set; }

        public DbSet<TeamStatsInfoModel> TeamStatsInfoModels { get; set; }

        public DbSet<PlayerStatsModel> PlayerStatsModels { get; set; }
        public DbSet<MatchInfoSummonerInfo> MatchInfoSummonerInfos { get; set; }
        
        public DbSet<MatchTimelineModel> MatchTimelineModels { get; set; }
        
        public DbSet<MapPositionModel> MatchPositionModels { get; set; }

        public DbSet<MatchEventModel> MatchEventModels { get; set; }
        
        public DbSet<MatchFrameModel> MatchFrameModels { get; set; }
        
        public DbSet<MatchParticipantFrameModel> MatchParticipantFrameModels { get; set; }

        // Implementace M : N pro summoner a match pomoci fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
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