using dotNetMVCLeagueApp.Repositories.SummonerInfo;
using dotNetSpLeagueApp.Repositories.SummonerInfo;

namespace dotNetMVCLeagueApp.Services {
    public class SummonerInfoService {

        private SummonerInfoRepository summonerInfoRepository;
        
        public SummonerInfoService(SummonerInfoRepository summonerInfoRepository) {
            this.summonerInfoRepository = summonerInfoRepository;
        }
    }
}