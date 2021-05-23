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
    public class ProfileCardService {
        private readonly ProfileCardRepository profileCardRepository;
        private readonly ApplicationUserRepository applicationUserRepository;

        public readonly int CardLimitPerUser = ServerConstants.CardLimit;

        public readonly List<string> SocialMediaUrlPrefixes = ServerConstants.SocialMediaPlatformPrefixes;

        public readonly List<string> SocialMediaPlatformNames = ServerConstants.SocialMediaPlatformsNames;

        public readonly Dictionary<string, string> SocialMedia = ServerConstants.SocialMedia;

        public ProfileCardService(ProfileCardRepository profileCardRepository,
            ApplicationUserRepository applicationUserRepository) {
            this.profileCardRepository = profileCardRepository;
            this.applicationUserRepository = applicationUserRepository;
        }

        public async Task<ProfileCardModel> Add(ProfileCardModel profileCard, bool showOnTop, ApplicationUser user) {
            var userCards = await profileCardRepository.GetUserProfileCardsByPosition(user);
            if (userCards.Count >= CardLimitPerUser) {
                throw new ActionNotSuccessfulException(
                    "Error, you cannot create more cards and must delete some first.");
            }

            return await profileCardRepository.AddProfileCardToCollection(profileCard, userCards, showOnTop);
        }

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

        public async Task<List<ProfileCardModel>> DeleteCard(int cardId, ApplicationUser user) =>
            await profileCardRepository.DeleteCard(cardId, user);

        public async Task<ProfileCardModel> GetCard(int cardId, ApplicationUser user) =>
            await profileCardRepository.Get(cardId, user);

        public async Task UpdateCard(ProfileCardModel card) =>
            await profileCardRepository.Update(card);
    }
}