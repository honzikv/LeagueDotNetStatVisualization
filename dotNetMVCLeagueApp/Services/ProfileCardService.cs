using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;

namespace dotNetMVCLeagueApp.Services {
    
    /// <summary>
    /// Sluzba pro operace s kartami na profilu
    /// </summary>
    public class ProfileCardService {
        private readonly ProfileCardRepository profileCardRepository;
        private readonly ApplicationUserRepository applicationUserRepository;

        /// <summary>
        /// Max limit karet pro uzivatele
        /// </summary>
        public readonly int CardLimitPerUser = ServerConstants.CardLimit;

        public readonly List<string> SocialMediaUrlPrefixes = ServerConstants.SocialMediaPlatformPrefixes;

        public readonly List<string> SocialMediaPlatformNames = ServerConstants.SocialMediaPlatformsNames;

        public readonly Dictionary<string, string> SocialMedia = ServerConstants.SocialMedia;

        public ProfileCardService(ProfileCardRepository profileCardRepository,
            ApplicationUserRepository applicationUserRepository) {
            this.profileCardRepository = profileCardRepository;
            this.applicationUserRepository = applicationUserRepository;
        }

        /// <summary>
        /// Prida kartu k danemu uzivateli
        /// </summary>
        /// <param name="profileCard">reference na kartu</param>
        /// <param name="showOnTop">zda-li ma byt karta na vrchu</param>
        /// <param name="user">refernece na uzivatele (vlastnika karty)</param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<ProfileCardModel> Add(ProfileCardModel profileCard, bool showOnTop, ApplicationUser user) {
            var userCards = await profileCardRepository.GetUserProfileCardsByPosition(user);
            if (userCards.Count >= CardLimitPerUser) {
                throw new ActionNotSuccessfulException(
                    "Error, you cannot create more cards and must delete some first.");
            }

            return await profileCardRepository.AddProfileCardToCollection(profileCard, userCards, showOnTop);
        }

        /// <summary>
        /// Posune kartu smerem nahoru
        /// </summary>
        /// <param name="profileCardId">id karty</param>
        /// <param name="user">vlastnik karty</param>
        /// <returns>seznam karet po posunu</returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<List<ProfileCardModel>> MoveUp(int profileCardId, ApplicationUser user) {
            var profileCards = await profileCardRepository.GetUserProfileCardsByPosition(user);
            var card = profileCards.FirstOrDefault(profileCard => profileCard.Id == profileCardId);

            if (card is null) {
                throw new ActionNotSuccessfulException("Error, card does not exist");
            }

            if (card.Position == 0) {
                return profileCards;
            }

            // Ziskame kartu na predchozi pozici
            var previousCard = profileCards[card.Position - 1];
            var swap = card.Position;
            card.Position = previousCard.Position;
            previousCard.Position = swap;

            await profileCardRepository.UpdateSwappedPositions(card, previousCard);

            // Nyni jeste potrebujeme zmenit pozice
            profileCards[card.Position] = card;
            profileCards[previousCard.Position] = previousCard;

            return profileCards;
        }

        /// <summary>
        /// Posune kartu smerem dolu
        /// </summary>
        /// <param name="profileCardId">Id karty</param>
        /// <param name="user">vlastnik karty</param>
        /// <returns>seznam karet po posunu</returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<List<ProfileCardModel>> MoveDown(int profileCardId, ApplicationUser user) {
            var profileCards = await profileCardRepository.GetUserProfileCardsByPosition(user);
            var card = profileCards.FirstOrDefault(profileCard => profileCard.Id == profileCardId);

            if (card is null) {
                throw new ActionNotSuccessfulException("Error, card does not exist");
            }

            if (card.Position == profileCards.Count - 1) {
                return profileCards;
            }

            // Ziskame kartu na predchozi pozici
            var followingCard = profileCards[card.Position + 1];
            var swap = card.Position;
            card.Position = followingCard.Position;
            followingCard.Position = swap;


            await profileCardRepository.UpdateSwappedPositions(card, followingCard);

            // Nyni jeste potrebujeme zmenit pozice
            profileCards[card.Position] = card;
            profileCards[followingCard.Position] = followingCard;

            return profileCards;
        }

        /// <summary>
        /// Ziska seznam karet pro summonera a seradi je podle pozice
        /// </summary>
        /// <param name="summoner">summoner</param>
        /// <returns>Seznam karet - pokud existuji, nebo prazdny seznam</returns>
        public async Task<List<ProfileCardModel>> GetProfileCardsForSummonerByPosition(SummonerModel summoner) {
            var linkedProfile = await applicationUserRepository.GetUserForSummoner(summoner);

            if (linkedProfile is null) {
                return new();
            }

            // Seradime podle pozice a vratime
            return linkedProfile.ProfileCards.OrderBy(card => card.Position).ToList();
        }

        /// <summary>
        /// Zkontroluje, zda-li je URL link na socialni sit validni
        /// </summary>
        /// <returns>
        /// Operation result, ktery obsahuje true pokud byla operace provedena s chybou a zpravu,
        /// jinak false bez zpravy
        /// </returns>
        public OperationResult<string> IsSocialNetworkValid(string userUrl) =>
            SocialMediaUrlPrefixes.Any(media =>
                userUrl.StartsWith(media, StringComparison.InvariantCultureIgnoreCase))
                ? new OperationResult<string>()
                : new(true, "Error, this URL is not supported.");

        /// <summary>
        /// Odstrani kartu z databaze
        /// </summary>
        /// <param name="cardId">id karty</param>
        /// <param name="user">vlastnik karty</param>
        /// <returns>Seznam karet po odstraneni</returns>
        public async Task<List<ProfileCardModel>> DeleteCard(int cardId, ApplicationUser user) =>
            await profileCardRepository.DeleteCard(cardId, user);

        /// <summary>
        /// Ziska kartu podle jejiho id a vlastnika
        /// </summary>
        /// <param name="cardId">id karty</param>
        /// <param name="user">vlastnik karty</param>
        /// <returns>Kartu nebo null</returns>
        public async Task<ProfileCardModel> GetCard(int cardId, ApplicationUser user) =>
            await profileCardRepository.Get(cardId, user);

        /// <summary>
        /// Aktualizuje kartu v db
        /// </summary>
        /// <param name="card">karta, kterou chceme aktualizovat</param>
        public async Task UpdateCard(ProfileCardModel card) =>
            await profileCardRepository.Update(card);
    }
}