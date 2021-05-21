using System.Threading.Tasks;
using dotNetMVCLeagueApp.Repositories;

namespace dotNetMVCLeagueApp.Services {
    public class ApplicationUserService {

        private readonly ApplicationUserRepository applicationUserRepository;
        public ApplicationUserService(ApplicationUserRepository applicationUserRepository) {
            this.applicationUserRepository = applicationUserRepository;
        }

        public async Task<bool> IsEmailTaken(string email) =>
            await applicationUserRepository.IsEmailTaken(email);
    }
}