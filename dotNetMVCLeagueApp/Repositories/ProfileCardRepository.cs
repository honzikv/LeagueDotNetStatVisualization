using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    public class ProfileCardRepository : EfCoreEntityRepository<ProfileCardModel, LeagueDbContext> {
        public ProfileCardRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        public async Task<ProfileCardModel> GetCardWithPosition(int targetPosition, ApplicationUser applicationUser) =>
            await LeagueDbContext.ProfileCardModels.Where(card =>
                    card.Position == targetPosition && card.ApplicationUser.Id == applicationUser.Id)
                .FirstOrDefaultAsync();

        /// <summary>
        /// Ziska z pole obsahujici id karet prvky a seradi je podle Id
        /// </summary>
        /// <param name="ids">Pole s id</param>
        /// <param name="user">Uzivatel, pro ktereho karty hledame</param>
        /// <returns>Seznam s entitami</returns>
        public async Task<List<ProfileCardModel>> GetProfileCardsWithIdsSortedById(IEnumerable<int> ids,
            ApplicationUser user) =>
            await LeagueDbContext.ProfileCardModels
                .Where(card => ids.Contains(card.Id) && card.ApplicationUser.Id == user.Id)
                .OrderBy(card => card.Id)
                .ToListAsync();
    }
}