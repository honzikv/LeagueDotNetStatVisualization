using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Data {
    public class LeagueDbContext : DbContext {
        public LeagueDbContext(DbContextOptions<LeagueDbContext> options)
            : base(options) {
        }

        public DbSet<SummonerInfoModel> SummonerInfoModels { get; set; }

        public DbSet<MatchInfoModel> MatchInfoModels { get; set; }

        public DbSet<ChampionBanModel> ChampionBanModels { get; set; }
    }
}