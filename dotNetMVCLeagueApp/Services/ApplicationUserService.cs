using System.Threading.Tasks;
using dotNetMVCLeagueApp.Repositories;

namespace dotNetMVCLeagueApp.Services {
    public class ApplicationUserService {

        private readonly ApplicationUserRepository applicationUserRepository;
        public ApplicationUserService(ApplicationUserRepository applicationUserRepository) {
            this.applicationUserRepository = applicationUserRepository;
        }

        /// <summary>
        /// Zjisti, zda-li je email zabrany
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>True, pokud je email zabrany</returns>
        public async Task<bool> IsEmailTaken(string email) =>
            await applicationUserRepository.IsEmailTaken(email);
    }
}