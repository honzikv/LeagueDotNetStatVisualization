using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    public class ApplicationUserRepository : EfCoreEntityRepository<ApplicationUser, LeagueDbContext> {
        public ApplicationUserRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        public async Task<bool> IsSummonerTaken(SummonerModel summoner) =>
            await LeagueDbContext.Users.AnyAsync(user => user.Summoner.Id == summoner.Id);
    }
}