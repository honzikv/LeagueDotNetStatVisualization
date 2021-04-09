using LeagueStatAppReact.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeagueStatAppReact.Controllers {
    public class SummonerInfoController : Controller {
        private readonly SummonerInfoService summonerInfoService;

        private readonly MatchHistoryService matchHistoryService;

        public SummonerInfoController(SummonerInfoService summonerInfoService,
            MatchHistoryService matchHistoryService) {
            this.summonerInfoService = summonerInfoService;
            this.matchHistoryService = matchHistoryService;
        }

  
        
    }
}