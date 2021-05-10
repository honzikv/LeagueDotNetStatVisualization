﻿using System;
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

        public DbSet<MapPositionModel> MapPositionModels { get; set; }

        public DbSet<MatchEventModel> MatchEventModels { get; set; }

        public DbSet<MatchFrameModel> MatchFrameModels { get; set; }

        public DbSet<MatchParticipantFrameModel> MatchParticipantFrameModels { get; set; }

        // Implementace M : N pro summoner a match pomoci fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MatchToSummonerModel>()
                .HasKey(matchSummoner => new {
                    matchSummoner.MatchInfoModelId,
                    matchSummoner.SummonerInfoModelId
                });

            modelBuilder.Entity<MatchToSummonerModel>()
                .HasOne(matchSummoner => matchSummoner.Match)
                .WithMany(matchInfo => matchInfo.SummonerInfoList)
                .HasForeignKey(matchInfo => matchInfo.MatchInfoModelId);
        }
    }
}