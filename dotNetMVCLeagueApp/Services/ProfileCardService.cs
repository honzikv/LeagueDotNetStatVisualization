using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils.Exceptions;

namespace dotNetMVCLeagueApp.Services {
    public class ProfileCardService {
        private readonly ProfileCardRepository profileCardRepository;

        private const int CardLimitPerUser = ServerConstants.CardLimit;

        public ProfileCardService(ProfileCardRepository profileCardRepository) {
            this.profileCardRepository = profileCardRepository;
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


        public async Task<List<ProfileCardModel>> DeleteCard(int cardId, ApplicationUser user) {
            return await profileCardRepository.DeleteCard(cardId, user);
        }

    }
}