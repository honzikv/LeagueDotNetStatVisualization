using System;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Data {
    /// <summary>
    ///     Kontext pro pripojeni k databazi
    /// </summary>
    public class LeagueDbContext : IdentityDbContext<ApplicationUser> {
        public LeagueDbContext(DbContextOptions<LeagueDbContext> options)
            : base(options) { }

        /// <summary>
        ///     Vsechny tabulky
        /// </summary>
        public DbSet<SummonerModel> SummonerModels { get; set; }
        
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<QueueInfoModel> QueueInfoModels { get; set; }

        public DbSet<ProfileCardModel> ProfileCardModels { get; set; }

        public DbSet<MatchModel> MatchModels { get; set; }

        public DbSet<PlayerModel> PlayerModels { get; set; }

        public DbSet<TeamStatsModel> TeamStatsModels { get; set; }

        public DbSet<PlayerStatsModel> PlayerStatsModels { get; set; }
        public DbSet<MatchToSummonerModel> MatchToSummonerModels { get; set; }

        public DbSet<MatchTimelineModel> MatchTimelineModels { get; set; }

        public DbSet<MatchEventModel> MatchEventModels { get; set; }

        public DbSet<MatchFrameModel> MatchFrameModels { get; set; }

        public DbSet<MatchParticipantFrameModel> MatchParticipantFrameModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder); // pro Identity
            
            // Implementace M : N pro summoner a match pomoci fluent API
            modelBuilder.Entity<MatchToSummonerModel>()
                .HasKey(matchSummoner => new {
                    MatchInfoModelId = matchSummoner.MatchModelId, SummonerInfoModelId = matchSummoner.SummonerModelId
                });

            modelBuilder.Entity<MatchToSummonerModel>()
                .HasOne(matchSummoner => matchSummoner.Match)
                .WithMany(matchInfo => matchInfo.SummonerInfoList)
                .HasForeignKey(matchInfo => matchInfo.MatchModelId);

        }
    }
}