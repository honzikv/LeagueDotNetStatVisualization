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

        public async Task<List<ProfileCardModel>> UpdateProfileCards(int[] ids, ApplicationUser user) {
            if (ids.IsNullOrEmpty()) {
                return new();
            }

            var profileCards = await profileCardRepository.GetProfileCardsWithIdsSortedById(ids, user);
            if (profileCards.Count != ids.Length) {
                throw new ActionNotSuccessfulException(
                    "Error, cannot reorder profile cards as some may have been deleted");
            }

            for (var i = 0; i < ids.Length; i += 1) { // i je pozice v seznamu
                var id = ids[i]; // id entity

                profileCards[id].Position = i;
            }

            return await profileCardRepository.UpdateAll(profileCards);
        }

        public async Task<List<ProfileCardModel>> DeleteCard(int cardId, ApplicationUser user) {
            return await profileCardRepository.DeleteCard(cardId, user);
        }
        
    }
}