using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Utils.Exceptions;
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

        /// <summary>
        /// Ziska karty a seradi je podle pozice
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<ProfileCardModel>> GetUserProfileCardsByPosition(ApplicationUser user) =>
            await LeagueDbContext.ProfileCardModels
                .Where(card => card.ApplicationUser.Id == user.Id)
                .OrderBy(card => card.Position)
                .ToListAsync();

        public async Task<List<ProfileCardModel>> DeleteCard(int cardId, ApplicationUser user) {
            var cardToDelete = await
                LeagueDbContext.ProfileCardModels.Where(card =>
                    card.Id == cardId && card.ApplicationUser.Id == user.Id).FirstOrDefaultAsync();

            if (cardToDelete is null) {
                throw new ActionNotSuccessfulException("Error, card does not exist");
            }

            var from = cardToDelete.Position; // budeme brat vsechny karty od pozice aktualni + 1
            var cardsAfterDeleted = await LeagueDbContext.ProfileCardModels.Where(card =>
                card.ApplicationUser.Id == user.Id && card.Position > from).ToListAsync();

            // Nyni pro kazdou kartu snizime jeji pozici o 1 a tim se zbavime "mezery" po karte, kterou smazeme
            cardsAfterDeleted.ForEach(card => card.Position -= 1);
            LeagueDbContext.Remove(cardToDelete);
            LeagueDbContext.UpdateRange(cardsAfterDeleted);
            await LeagueDbContext.SaveChangesAsync();

            return await GetUserProfileCardsByPosition(user);
        }

        public async Task UpdateSwappedPositions(ProfileCardModel card1,
            ProfileCardModel card2) {
            LeagueDbContext.UpdateRange(card1, card2);
            await LeagueDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Prida profilovou kartu do kolekce. Musi mit jiz prirazeny ApplicationUser objekt, jinak
        /// rozbije databazi.
        /// </summary>
        /// <param name="profileCard">Karta, ktera se ma pridat</param>
        /// <param name="profileCards">Seznam vsech karet</param>
        /// <param name="showOnTop">Zda-li se ma pridat jako prvni</param>
        /// <returns></returns>
        public async Task<ProfileCardModel> AddProfileCardToCollection(ProfileCardModel profileCard,
            List<ProfileCardModel> profileCards, bool showOnTop) {
            if (showOnTop) {
                profileCard.Position = 0;
                profileCards.ForEach(card => card.Position += 1);
                LeagueDbContext.UpdateRange(profileCards);
            }
            else {
                profileCard.Position = profileCards.LastOrDefault()?.Position + 1 ?? 0;
            }

            LeagueDbContext.Add(profileCard);
            await LeagueDbContext.SaveChangesAsync();
            return profileCard;
        }

        /// <summary>
        /// Ziska kartu podle id a uzivatele
        /// </summary>
        /// <param name="cardId">Id karty</param>
        /// <param name="user">id uzivatele</param>
        /// <returns></returns>
        public async Task<ProfileCardModel> Get(int cardId, ApplicationUser user) => await
            LeagueDbContext.ProfileCardModels.Where(card => card.Id == cardId && card.ApplicationUser.Id == user.Id)
                .FirstOrDefaultAsync();
    }
}