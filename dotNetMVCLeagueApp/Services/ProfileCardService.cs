using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Areas.Identity.Data;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils.Exceptions;

namespace dotNetMVCLeagueApp.Services {
    public class ProfileCardService {
        private readonly ProfileCardRepository profileCardRepository;

        public ProfileCardService(ProfileCardRepository profileCardRepository) {
            this.profileCardRepository = profileCardRepository;
        }

        public async Task<ProfileCardModel> Add(ProfileCardModel profileCardModel) {
            return null;
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
            
            for (var i = 0; i < ids.Length; i += 1) {
                var id = ids[i]; // id entity
                var position = i; // pozice

                profileCards[id].Position = position;
            }

            return await profileCardRepository.UpdateAll(profileCards);
        }

        public async Task<List<ProfileCardModel>> DeleteCard(int cardId, ApplicationUser user) {
            return null;
        }
    }
    
    
}