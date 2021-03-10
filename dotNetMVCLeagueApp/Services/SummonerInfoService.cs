using dotNetSpLeagueApp.Repositories.SummonerInfo;

namespace dotNetSpLeagueApp.Services {
    public class SummonerInfoService {

        private SummonerInfoRepository summonerInfoRepository;

        public SummonerInfoService(SummonerInfoRepository summonerInfoRepository) {
            this.summonerInfoRepository = summonerInfoRepository;
        }
    }
}