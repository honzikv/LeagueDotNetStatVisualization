using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    
    /// <summary>
    /// Repozitar pro uzivatele v aplikaci
    /// </summary>
    public class ApplicationUserRepository : EfCoreEntityRepository<ApplicationUser, LeagueDbContext> {
        public ApplicationUserRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        /// <summary>
        /// Zda-li je summoner zabrany
        /// </summary>
        /// <param name="summoner"></param>
        /// <returns></returns>
        public async Task<bool> IsSummonerTaken(SummonerModel summoner) =>
            await LeagueDbContext.Users.AnyAsync(user => user.Summoner.Id == summoner.Id);

        public async Task<bool> IsEmailTaken(string email) =>
            await LeagueDbContext.Users.AnyAsync(user => user.EmailConfirmed &&
                                                         user.Email.ToLower() == email.ToLower());

        public async Task<ApplicationUser> GetUserForSummoner(SummonerModel summoner) =>
            await LeagueDbContext.Users
                .Where(user => user.Summoner != null && user.Summoner.EncryptedAccountId == summoner.EncryptedAccountId)
                .FirstOrDefaultAsync();
    }
}