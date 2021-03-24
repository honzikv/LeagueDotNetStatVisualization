using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Services;
using Microsoft.AspNetCore.Mvc;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerInfoController : Controller {
        private readonly SummonerInfoService summonerInfoService;

        private readonly MatchHistoryService matchHistoryService;

        public SummonerInfoController(SummonerInfoService summonerInfoService,
            MatchHistoryService matchHistoryService) {
            this.summonerInfoService = summonerInfoService;
            this.matchHistoryService = matchHistoryService;
        }


        public IActionResult Index() {
            return View();
        }

    }
}